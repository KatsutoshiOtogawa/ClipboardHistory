using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Linq;
using Windows.ApplicationModel.Contacts;

namespace OtogawaKatsutoshi.Powershell.Commands
{

    public class RemoveClipboardHistoryCommand
    {

        public async static Task<bool[]> RemoveClipboardHistoryContentAsText(int[] indexes = null, bool skip = false, bool all = false)
        {


            if (indexes.Any(index => index < 0)) {
                throw new IndexOutOfRangeException();
            }

            if (indexes == null && all)
            {
                if(!Clipboard.ClearHistory())
                {
                    // clipboardのヒストリーが削除できなかった。
                    throw new ArgumentException();
                }
                // zennbu true
                return (new List<bool>((await Clipboard.GetHistoryItemsAsync()).Items.Count)).ToArray();
            } else if (indexes != null && all)
            {
                throw new ArgumentException();
            }

            // indexが与えられているパターンのみ
            var items = await GetClipboardHistoryCommand.GetClipboardHistoryContentAsItem(indexes, skip);

            // 長さが０の削除も正常。
            if (items.Length == 0)
            {

                return List<bool>(0);
            }

            //Clipboard.DeleteItemFromHistory

            var result = items.Select(element => Clipboard.DeleteItemFromHistory(element))
                .ToArray();

            string[] clipboardItems;
            //List<string> clipboardItems;

            if (indexes == null)
            {

                //Content.GetTextAsync(); clipboardItems = clipboardhistory.Items
                //    .Select(async element => await element.Content.GetTextAsync())
                //    .ToArray();
                clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();
            } else
            {

                clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                            .Where((element, index) => indexes.Contains(index) ^ skip)
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();
                //// index
                //clipboardhistory.Items
                //    .Where((element, index) => indexes.Contains(index))
                //    .Select(element => element.Content.GetTextAsync())
                //    .ToArray();
            }

            return clipboardItems;

        }
    }

    public class GetClipboardHistoryCommand
    {

        // csharp7.3 canonot usse optional type reference.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">clipboard historys index. start zero. if this parameter assinged null, all element return .</param>
        /// <param name="skip">if this parameter assinged true, all element return index.</param>
        /// 
        /// <returns></returns>
        public async static Task<string[]> GetClipboardHistoryContentAsText(int[] indexes = null, bool skip = false)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            if (indexes.Any(index => index < 0)) {
                throw new IndexOutOfRangeException();
            }

            string[] clipboardItems;
            //List<string> clipboardItems;

            if (indexes == null)
            {

                clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();
            } else
            {

                clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                            .Where((element, index) => indexes.Contains(index) ^ skip)
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();
            }

            return clipboardItems;
        }

        public async static Task<ClipboardHistoryItem[]> GetClipboardHistoryContentAsItem(int[] indexes = null, bool skip = false)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            if (indexes.Any(index => index < 0)) {
                throw new IndexOutOfRangeException();
            }

            ClipboardHistoryItem[] clipboardItems;
            //List<string> clipboardItems;

            if (indexes == null)
            {

                clipboardItems = clipboardhistory.Items.ToArray();
            } else
            {

                clipboardItems = clipboardhistory.Items
                            .Where((element, index) => indexes.Contains(index) ^ skip)
                            .ToArray();
            }

            return clipboardItems;
        }

        public async static Task<string[]> GetClipboardHistoryContentAsText(int first = 0, int last = 0)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            //Cannot validate argument on parameter 'First'.The - 1 argument is less than the minimum allowed range of 0.Supply an argument that is greater than or equal to 0 and then try the command again.

            // firstもlastも大きい分には良い。
            // firstとlastを同時に選択できるが、
            // firstとskiplastみたいに両方を選択することはできない。
            // 0のときはは何も選択しない。

            if (first < 0 || last < 0)
            {
                throw new IndexOutOfRangeException();
            }


            string[] clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                            .Where((element, index) => index + 1 > first || clipboardhistory.Items.Count - index -1 < last)
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();

            return clipboardItems;
        }

        public async static Task<ClipboardHistoryItem[]> GetClipboardHistoryContentAsItem(int first = 0, int last = 0)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            //Cannot validate argument on parameter 'First'.The - 1 argument is less than the minimum allowed range of 0.Supply an argument that is greater than or equal to 0 and then try the command again.

            // firstもlastも大きい分には良い。
            // firstとlastを同時に選択できるが、
            // firstとskiplastみたいに両方を選択することはできない。
            // 0のときはは何も選択しない。

            if (first < 0 || last < 0)
            {
                throw new IndexOutOfRangeException();
            }


            ClipboardHistoryItem[] clipboardItems = 
                        clipboardhistory.Items
                            .Where((element, index) => index + 1 > first || clipboardhistory.Items.Count - index -1 < last)
                            .ToArray();

            return clipboardItems;
        }

        public async static Task<string[]> GetClipboardHistoryContentAsText(int range = 0, bool skip = false, bool first = false, bool last = false)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            // firstとlast両方選択されることはない。

            if (!(first ^ last))
            {
                throw new ArgumentException();
            }


            string[] clipboardItems = (
                        await Task.WhenAll(clipboardhistory.Items
                            .Where((element, index) => (index + 1 > range ^ (first && skip)))
                           .Select(async element => await element.Content.GetTextAsync()))
                     ).ToArray();

            return clipboardItems;
        }

        public async static Task<ClipboardHistoryItem[]> GetClipboardHistoryContentAsItem(int range = 0, bool skip = false, bool first = false, bool last = false)
        {

            var clipboardhistory = await Clipboard.GetHistoryItemsAsync();

            // firstとlast両方選択されることはない。

            if (!(first ^ last))
            {
                throw new ArgumentException();
            }


            ClipboardHistoryItem[] clipboardItems = 
                        clipboardhistory.Items
                            .Where((element, index) => (index + 1 > range ^ (first && skip)))
                            .ToArray();

            return clipboardItems;
        }
// skiplast skip index

// ラムダ式の上でcontainsを処理する。
// skiplast 最後の個を読まない。
// last 最後の子を読む。

// skipfiast　最初の数個読み飛ばす。
// first = 1 index

// -indexは配列をとれる。

 //

        //public async static Task<List<string>> GetClipboardHistoryContentAsText()
        //{
            
        //    var result = await Clipboard.GetHistoryItemsAsync();
        //    List<string> resultsItems = new List<string>(result.Items.Count);

        //    foreach (var val in result.Items) {
        //        resultsItems.Add(await val.Content.GetTextAsync());
        //    }
        //    return resultsItems;
        //    //             try
        //    // {
        //    //     textContent = Clipboard.GetText();
        //    // }
        //    // catch (PlatformNotSupportedException)
        //    // {
        //    //     ThrowTerminatingError(new ErrorRecord(new InvalidOperationException(ClipboardResources.UnsupportedPlatform), "FailedToGetClipboardUnsupportedPlatform", ErrorCategory.InvalidOperation, "Clipboard"));
        //    // }
        //}
    }
}
