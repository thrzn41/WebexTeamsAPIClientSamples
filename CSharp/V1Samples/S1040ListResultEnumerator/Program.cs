/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using SampleShared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thrzn41.Util;
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;

namespace S1040ListResultEnumerator
{

    /// <summary>
    /// Markdown Builder.
    /// </summary>
    class Program
    {

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">args of this app.</param>
        /// <returns>Task for async.</returns>
        static async Task MainAsync(string[] args)
        {

            /* ********************************************************
             * NOTE: THIS IS ONLY A SAMPLE.
             * I will put most codes in this Main() on purpose.
             * So, you will be able to understand the sample
             * after you read it from top to bottom.
             * You do not need to care about 'SampleUtil' in this code.
             * The 'SampleUtil' does something tedious.
             * Only you need to do to understand the sample
             * is reading this code in Main() from top to bottom. 
             * *********************************************************/

            SampleUtil.ShowTitle("[S1030] ListResult Enumerator(Pagination)", "Get first list result, and then get next list result...");


            // Load encrypted token that is encrypted by 'S0010SetupSamples'.
            ProtectedString token = SampleUtil.LoadEncryptedToken();

            if (token != null)
            {

                ////////////////////////////////////////////////////////////////////////////
                // Create an instance for Webex Teams API.
                // As best practice, the instance should be re-used as long as possible.
                // For bots, the lifetime of the instance typically is almost the same as the lifetime of the app process.
                var teams = TeamsAPI.CreateVersion1Client(token);


                /////////////////////////////////////////////////////
                // Create 4 temporary spaces for the sample.
                for (int i = 0; i < 4; i++)
                {
                    string title = String.Format("Sample Temporary Space-{0} #WebexTeamsAPIClientV1SamplesTemporary", i);


                    SampleUtil.ShowMessage("Create a Space: {0}", title);

                    var r = await teams.CreateSpaceAsync(title);

                    if(r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to create space.");
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to create space, StatusCode = {0}", r.HttpStatusCode);
                    }
                }


                /////////////////////////////////////////////////////
                // List spaces for each 2 spaces.
                // In this case, 'max: 2' parameter is set.
                var e = (await teams.ListSpacesAsync(
                            max: 2,
                            type: SpaceType.Group,
                            sortBy: SpaceSortBy.Created)
                        ).GetListResultEnumerator();
                
                // In this sample, give up iteration after getting 4 spaces.
                int count = 4;

                // Iterate list result.
                while(await e.MoveNextAsync())
                {
                    // Get current result.
                    var r = e.CurrentResult;

                    if(r.IsSuccessStatus && r.Data.HasItems)
                    {
                        SampleUtil.ShowMessage("Succeeded to get {0} spaces.", r.Data.ItemCount);

                        foreach (var space in r.Data.Items)
                        {
                            SampleUtil.ShowMessage("  Title: {0}", space.Title);
                            count--;
                        }
                    }

                    if(count <= 0)
                    {
                        break;
                    }
                }




                /////////////////////////////////////////////////////
                // Clean up the temporary created spaces.
                SampleUtil.ShowMessage("Cleaning up the temporary created spaces.");

                var temporarySpaces = new List<Space>();

                /////////////////////////////////////////////////////
                // Try to find all the temporary created spaces.
                e = (await teams.ListSpacesAsync(type: SpaceType.Group)).GetListResultEnumerator();

                while(await e.MoveNextAsync())
                {
                    var r = e.CurrentResult;

                    if(r.IsSuccessStatus && r.Data.HasItems)
                    {
                        foreach (var space in r.Data.Items)
                        {
                            if(space.Title.Contains("#WebexTeamsAPIClientV1SamplesTemporary"))
                            {
                                temporarySpaces.Add(space);
                            }
                        }
                    }
                }

                /////////////////////////////////////////////////////
                // Delete the temporary created spaces found.
                foreach (var space in temporarySpaces)
                {
                    SampleUtil.ShowMessage("Deleting {0}", space.Title);

                    var r = await teams.DeleteSpaceAsync(space);

                    if (r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to delete space.");
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to delete space, StatusCode = {0}", r.HttpStatusCode);
                    }
                }


            }
        }




        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">args for this app.</param>
        static void Main(string[] args)
        {
            try
            {
                // If you use C# 7.1 or later, you can simply use async Main().
                // In this sample, 'static async Task MainAsync(string[] args)' is implemented instead of using 'async Main()'.
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                SampleUtil.ShowError(ex);
            }

            SampleUtil.WaitKeyPressToExit();
        }

    }
}
