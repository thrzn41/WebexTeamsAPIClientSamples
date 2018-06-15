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
using System.Threading.Tasks;
using Thrzn41.Util;
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;

namespace S1020CheckSucceededOrNot
{

    /// <summary>
    /// Check if a request suceeded or not.
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

            SampleUtil.ShowTitle("[S1020] Check if a request suceeded or not", "Sample to check if a request suceeded or not, or handle TeamsResultException.");


            // Load encrypted token that is encrypted by 'S0010SetupSamples'.
            ProtectedString token = SampleUtil.LoadEncryptedToken();

            if (token != null)
            {

                ////////////////////////////////////////////////////////////////////////////
                // Create an instance for Webex Teams API.
                // As best practice, the instance should be re-used as long as possible.
                // For bots, the lifetime of the instance typically is almost the same as the lifetime of the app process.
                var teams = TeamsAPI.CreateVersion1Client(token);

                // Try to find Sample space.
                var space = await SampleUtil.FindSampleSpaceAsync(teams);


                if (space != null)
                {

                    /////////////////////////////////////////////////////////////////////
                    // result.IsSuccessStatus indicates the request succeeded or not.
                    var r = await teams.CreateMessageAsync(space, "This message will be posted.");

                    if (r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to post a message: Id = {0}", r.Data.Id);
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to post a message: StatusCode = {0}, TrackingId = {1}", r.HttpStatusCode, r.TrackingId);
                    }


                    /////////////////////////////////////////////////////////////////////
                    // This request will not succeed because the space id is invalid.
                    r = await teams.CreateMessageAsync("invalid_space_id", "Bacause of invalid space id, this message will not be posted.");

                    if (r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to post a message: Id = {0}", r.Data.Id);
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to post a message: StatusCode = {0}, TrackingId = {1}", r.HttpStatusCode, r.TrackingId);
                    }


                    ////////////////////////////////////////////////////////////////////////
                    // result.GetData() throws TeamsResultException on error response.
                    // On the other hands, result.Data does not throw TeamsResultException.
                    try
                    {
                        r = await teams.CreateMessageAsync("invalid_space_id", "Bacause of invalid space id, this message will not be posted.");


                        /////////////////////////////////////////////////////
                        // This does not throw TeamsResultException.
                        // And empty id will be shown.
                        var message = r.Data;
                        SampleUtil.ShowMessage("Message id by r.Data.Id = {0}", message.Id);


                        /////////////////////////////////////////////////////
                        // This throws TeamsResultException.
                        // So, the id will not be shown.
                        message = r.GetData();
                        SampleUtil.ShowMessage("Message id by r.GetData().Id = {0}", message.Id);

                    }
                    catch (TeamsResultException tre)
                    {
                        SampleUtil.ShowMessage("{0}: StatusCode = {1}, TrackingId = {2}",
                            tre.Message,
                            tre.HttpStatusCode,
                            tre.TrackingId);
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
