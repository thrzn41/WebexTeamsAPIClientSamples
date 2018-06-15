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
using System.IO;
using System.Threading.Tasks;
using Thrzn41.Util;
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;

namespace S0010SetupSamples
{

    /// <summary>
    /// Setup for samples.
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

            SampleUtil.ShowTitle("[S0010] Setup for samples", "Encrypt Bot token and find or create a space for the samples.");


            //////////////////////////////////
            // Read bot token from you.
            Console.WriteLine("Please copy and paste Bot token you want to use in the samples.");
            Console.WriteLine("And then, press enter key.");
            Console.Write("Bot token here> ");

            string token = Console.ReadLine();

            if (!String.IsNullOrEmpty(token))
            {
                //////////////////////////////////
                // Encrypt the token.
                var encryptedToken = LocalProtectedString.FromString(token);


                //////////////////////////////////
                // Check if the token is for bot.
                var teams = TeamsAPI.CreateVersion1Client(encryptedToken);

                var rMe = await teams.GetMeFromCacheAsync();

                if (rMe.IsSuccessStatus)
                {
                    var me = rMe.Data;

                    Console.WriteLine("-------");
                    Console.WriteLine("Name: {0}", me.DisplayName);
                    Console.WriteLine("Type: {0}", me.TypeName);
                    Console.WriteLine("-------");

                    if (me.Type != PersonType.Bot)
                    {
                        Console.WriteLine("The person is not Bot.");
                        Console.WriteLine("For most samples, the Bot account is strongly recommended.");

                        if(!SampleUtil.WaitKeyPress("Press 'y' if you want to proceed with 'Non'-bot account. Press other key to cancel.", 'y'))
                        {
                            SampleUtil.ShowMessage("Setup canceled.");
                            return;
                        }
                    }


                    //////////////////////////////////
                    // Export encrypted token data.
                    var    dir  = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    string path = dir.CreateSubdirectory(".thrzn41").CreateSubdirectory("WebexTeamsAPIClientSamples").CreateSubdirectory("V1Samples").FullName;

                    using (var fs = new FileStream(String.Format("{0}{1}token.dat", path, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        await fs.WriteAsync(encryptedToken.EncryptedData, 0, encryptedToken.EncryptedData.Length);
                    }

                    using (var fs = new FileStream(String.Format("{0}{1}entropy.dat", path, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        await fs.WriteAsync(encryptedToken.Entropy, 0, encryptedToken.Entropy.Length);
                    }

                    Console.WriteLine("Encrypted token was exported: Path = {0}", path);


                    //////////////////////////////////////////////////////////
                    // Read Webex Teams account to add to the sample space.
                    Console.WriteLine();
                    Console.WriteLine("This app will create or find the space for the sample, and then add your Webex Teams account to the space.");
                    Console.WriteLine("Please enter your Webex Teams account(email address style).");
                    Console.Write("Enter Webex Teams account> ");

                    string teamsAccount = Console.ReadLine();

                    if (!String.IsNullOrEmpty(teamsAccount))
                    {

                        Console.WriteLine();
                        Console.WriteLine("Please confirm the Webex Teams account carefully.");
                        Console.WriteLine("This account will be added to the space for the sample.");
                        Console.WriteLine();
                        Console.WriteLine("Entered account: {0}", teamsAccount);

                        if (!SampleUtil.WaitKeyPress("Press 'y' if your Webex Teams account is correct. Press other key to cancel.", 'y'))
                        {
                            SampleUtil.ShowMessage("Setup canceled.");
                            return;
                        }

                        //////////////////////////////////
                        // Try to find the sample space.
                        Space spaceForSample = null;

                        var e = (await teams.ListSpacesAsync()).GetListResultEnumerator();

                        while (await e.MoveNextAsync())
                        {
                            var rSpaces = e.CurrentResult;

                            if (rSpaces.IsSuccessStatus && rSpaces.Data.HasItems)
                            {
                                var spaces = rSpaces.Data;

                                foreach (var item in spaces.Items)
                                {
                                    if (item.Title.EndsWith("#WebexTeamsAPIClientV1SamplesSpace"))
                                    {
                                        spaceForSample = item;
                                        break;
                                    }
                                }
                            }

                            if (spaceForSample != null)
                            {
                                break;
                            }
                        }


                        //////////////////////////////////////////////////////////
                        // Create a new sample space when no sample space found.
                        if (spaceForSample == null)
                        {
                            var rSpace = await teams.CreateSpaceAsync("Webex Teams API Client Samples #WebexTeamsAPIClientV1SamplesSpace");

                            if (rSpace.IsSuccessStatus)
                            {
                                spaceForSample = rSpace.Data;
                            }
                        }

                        if (spaceForSample != null)
                        {
                            ////////////////////////////////////////////////////////
                            // Try to add Webex Teams account to the sample space.
                            var rSpaceMembership = await teams.CreateSpaceMembershipAsync(spaceForSample, teamsAccount);

                            if (rSpaceMembership.IsSuccessStatus || rSpaceMembership.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
                            {
                                SampleUtil.ShowMessage("{0} was added to Space for the samples", teamsAccount);
                            }
                            else
                            {
                                SampleUtil.ShowMessage("Failed to add {0} to Space for the samples: Error = {1}", teamsAccount, rSpaceMembership.HttpStatusCode);
                            }

                            Console.WriteLine("-------");
                            Console.WriteLine("Space: {0}", spaceForSample.Title);
                            Console.WriteLine("-------");
                        }
                        else
                        {
                            SampleUtil.ShowMessage("Failed to find or create Space for the samples.");
                            return;
                        }


                        SampleUtil.ShowMessage("Setup for the samples has completed.");
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Webex Teams account is null or empty.");
                        return;
                    }
                }
                else
                {
                    SampleUtil.ShowMessage("Failed to get person info from Webex Teams API service: Error = {0}", rMe.HttpStatusCode);
                    return;
                }
            }
            else
            {
                SampleUtil.ShowMessage("Token is null or empty.");
                return;
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
            catch(Exception ex)
            {
                SampleUtil.ShowError(ex);
            }

            SampleUtil.WaitKeyPressToExit();
        }

    }
}
