using System;
using System.IO;

namespace Slack_Dark_Mode
{
    class Program
    {
        static void Main(string[] args)
        {

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming","Local");
            var rootPath = @"slack";

            var versionPath = @"app-3.3.3";
            var folderPath = @"resources\app.asar.unpacked\src\static";

            var primary = "#09F";
            var text = "#ABB2BF";
            var background = "#080808";
            var backgroundElevated = "#222";

            var fullPath = Path.Combine(appDataPath, rootPath, versionPath, folderPath);
            var appendText = string.Format(@"
//DARK MODE ENABLE
// First make sure the wrapper app is loaded
document.addEventListener(""DOMContentLoaded"", function() {{

  // Then get its webviews
  let webviews = document.querySelectorAll("".TeamView webview"");

  // Fetch our CSS in parallel ahead of time
 // const cssPath = 'https://cdn.rawgit.com/widget-/slack-black-theme/master/custom.css';
 const cssPath = 'https://raw.githubusercontent.com/Nockiro/slack-black-theme/3ea2efdfb96ccc91549837ab237d57104181bbf8/custom.css';
  let cssPromise = fetch(cssPath).then(response => response.text());

  let customCustomCSS = `
  :root {{
     /* Modify these to change your theme colors: */
--primary: {0};
--text: {1};
--background: {2};
--background-elevated: {3};
  }}
  `

  // Insert a style tag into the wrapper view
  cssPromise.then(css => {{
     let s = document.createElement('style');
     s.type = 'text/css';
     s.innerHTML = css + customCustomCSS;
     document.head.appendChild(s);
  }});

  // Wait for each webview to load
  webviews.forEach(webview => {{
     webview.addEventListener('ipc-message', message => {{
        if (message.channel == 'didFinishLoading')
           // Finally add the CSS into the webview
           cssPromise.then(css => {{
              let script = `
                    let s = document.createElement('style');
                    s.type = 'text/css';
                    s.id = 'slack-custom-css';
                    s.innerHTML = \`${{css + customCustomCSS}}\`;
                    document.head.appendChild(s);
                    `
              webview.executeJavaScript(script);
           }})
     }});
  }});
}});", primary, text, background, backgroundElevated);

            var filePath1 = Path.Combine(fullPath, "ssb-interop.js");
            var filePath2 = Path.Combine(fullPath, "index.js");

#if DEBUG
            //debug info
            Console.WriteLine("\r\nfilePath1 = " + filePath1);
            Console.WriteLine("\r\nfilePath2 = " + filePath2);
            Console.WriteLine("\r\nappendText = " + appendText);
#endif

            Console.WriteLine("This application will convert your Slack windows desktop app to 'dark mode'");
            Console.WriteLine("If you don't want it changed - now's your chance to close it and pretend this never happened...");
            Console.WriteLine("\r\n If you DO want it, PRESS ANY KEY TO CONTINUE");
            Console.ReadLine();
            Console.WriteLine("here we go!");



            // This text is added only once to the file.
            if (File.Exists(filePath1))
            {
                // Backup previous file
                if (!File.Exists(filePath1 + "-BAK"))
                {
                    try
                    {
                        Console.WriteLine("Creating backup copy: " + filePath1 + "-BAK");
                        File.Copy(filePath1, filePath1 + "-BAK", false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating backup copy!");
                        Console.WriteLine("Exception: " + ex);
                        Console.WriteLine("-- -- -- -- -- --");
                        Console.WriteLine("Application will exit.");
                        Console.WriteLine("Press any key to continue...\r\n\r\n\r\n");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Backup file already exists");
                }

                //check to see if this has already been enabled
                using (StreamReader sr = new StreamReader(filePath1))
                {
                    var contents = sr.ReadToEnd();
                    if (contents.Contains("//DARK MODE ENABLE"))
                    {
                        Console.WriteLine("It looks like Dark Mode has already been enabled in ssb-interop.js. \r\n\r\n");
                       // Console.ReadLine();
                        //Environment.Exit(0);
                    }
                }

                // Create a file to write to.
                Console.WriteLine("Adding custom CSS to Slack program file: " + filePath1);
                using (StreamWriter sw = File.AppendText(filePath1))
                {
                    sw.WriteLine(appendText);
                }
            }

            if (File.Exists(filePath2))
            {
                // Backup previous file
                if (!File.Exists(filePath2 + "-BAK"))
                {
                    try
                    {
                        Console.WriteLine("Creating backup copy: " + filePath2 + "-BAK");
                        File.Copy(filePath2, filePath2 + "-BAK", false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating backup copy!");
                        Console.WriteLine("Exception: " + ex);
                        Console.WriteLine("-- -- -- -- -- --");
                        Console.WriteLine("Application will exit.");
                        Console.WriteLine("Press any key to continue...\r\n\r\n\r\n");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Backup file already exists");
                }


                //check to see if this has already been enabled
                using (StreamReader sr = new StreamReader(filePath2))
                {
                    var contents = sr.ReadToEnd();
                    if (contents.Contains("//DARK MODE ENABLE"))
                    {
                        Console.WriteLine("It looks like Dark Mode already been enabled in Index.js. \r\n \r\n Press any key to exit the application.");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }

                // Create a file to write to.
                Console.WriteLine("Adding custom CSS to Slack program file: " + filePath2);
                using (StreamWriter sw = File.AppendText(filePath2))
                {
                    sw.WriteLine(appendText);
                }
            }
            

            Console.WriteLine("\r\n\r\nApplication complete!");
            Console.WriteLine("If you would like to go back to the old settings, replace '" + filePath1 + "' and '" + filePath2 + "' with the same named files ending in -BAK.");
            Console.WriteLine("\r\n\r\n\r\nPlease completely quit Slack (Hamburger menu (upper left) --> File --> Quit Slack) and re-open to see the changes");
            Console.ReadLine();
        }
    }
}
