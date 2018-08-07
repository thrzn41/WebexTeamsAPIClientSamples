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

namespace S1030MarkdownBuilder
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

            SampleUtil.ShowTitle("[S1030] Markdown Builder", "How to use markdown builder.");


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

                    ////////////////////////////////////////////////////////////////
                    // You can build markdown that can be used in Webex Teams API.
                    var md = new MarkdownBuilder();

                    // Bold
                    md.Append("Hello, Bold ").AppendBold("WebexTeams").Append("!!").AppendLine();

                    // Italic
                    md.Append("Hello, Italic ").AppendItalic("WebexTeams").Append("!!").AppendLine();

                    // Link
                    md.AppendParagraphSeparater();
                    md.AppendBlockQuote("Hi!").AppendLink("This is Link", new Uri("https://www.google.com/")).Append(".");

                    // Block Quote
                    md.AppendParagraphSeparater();
                    md.AppendBlockQuote("Hi! This is Block Quote.");

                    // Ordered List
                    md.AppendParagraphSeparater();
                    md.AppendBold("This is Ordered list:").AppendLine();
                    md.AppendOrderedList("list item 01");
                    md.AppendOrderedList("list item 02");
                    md.AppendOrderedList("list item 03");

                    // Unordered List
                    md.AppendParagraphSeparater();
                    md.AppendBold("This is Unordered list:").AppendLine();
                    md.AppendUnorderedList("list item 01");
                    md.AppendUnorderedList("list item 02");
                    md.AppendUnorderedList("list item 03");

                    // Inline Code.
                    md.AppendParagraphSeparater();
                    md.Append("The ").AppendInLineCode("print(\"Hello, World!!\")").Append(" is inline code.");

                    // Code Block.
                    md.AppendParagraphSeparater();
                    md.Append("This is Code Block:").AppendLine();
                    md.BeginCodeBlock()
                        .Append("#include <stdio.h>\n")
                        .Append("\n")
                        .Append("int main(void)\n")
                        .Append("{\n")
                        .Append("    printf(\"Hello, World!!\\n\");\n")
                        .Append("\n")
                        .Append("    return 0;\n")
                        .Append("}\n")
                      .EndCodeBlock();

                    var r = await teams.CreateMessageAsync(space, md.ToString());

                    if (r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to post a message: Id = {0}", r.Data.Id);
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to post a message: StatusCode = {0}", r.HttpStatusCode);
                    }



                    // Mentioned to All.
                    md.Clear();
                    md.Append("Hi ").AppendMentionToAll().Append(", this message is mentioned to all in the space.");

                    r = await teams.CreateMessageAsync(space, md.ToString());

                    if (r.IsSuccessStatus)
                    {
                        SampleUtil.ShowMessage("Succeeded to post a message: Id = {0}", r.Data.Id);
                    }
                    else
                    {
                        SampleUtil.ShowMessage("Failed to post a message: StatusCode = {0}", r.HttpStatusCode);
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
