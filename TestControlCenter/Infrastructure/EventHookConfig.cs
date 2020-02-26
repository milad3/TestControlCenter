using EventHook;
using System;
using TestControlCenter.Tools;

namespace TestControlCenter.Infrastructure
{
    public class EventHookConfig
    {
        private static KeyboardWatcher keyboardWatcher;
        private static MouseWatcher mouseWatcher;
        private static ApplicationWatcher applicationWatcher;

        public static ExamTools ExamTools { get; set; }

        public static void Hook()
        {
            using (var eventHookFactory = new EventHookFactory())
            {
                keyboardWatcher = eventHookFactory.GetKeyboardWatcher();
                keyboardWatcher.Start();

                keyboardWatcher.OnKeyInput += async (s, e) =>
                {
                    if(!GlobalValues.ExamIsRunning)
                    {
                        return;
                    }

                    if(e.KeyData.EventType != KeyEvent.down)
                    {
                        return;
                    }

                    if(!IsKeyboardButtonValidForTakingScreenshot(e))
                    {
                        return;
                    }

                    await ExamTools.TakeRecord(keyName: e.KeyData.Keyname);
                };

                mouseWatcher = eventHookFactory.GetMouseWatcher();
                mouseWatcher.Start();
                mouseWatcher.OnMouseInput += async (s, e) =>
                {
                    if (!GlobalValues.ExamIsRunning)
                    {
                        return;
                    }

                    if (!IsMouseButtonValidForTakingScreenshot(e))
                    {
                        return;
                    }

                    await ExamTools.TakeRecord(isMouse: true);
                };

                //applicationWatcher = eventHookFactory.GetApplicationWatcher();
                //applicationWatcher.Start();
                //applicationWatcher.OnApplicationWindowChange += (s, e) =>
                //{
                //    Console.WriteLine(string.Format("Application window of '{0}' with the title '{1}' was {2}", e.ApplicationData.AppName, e.ApplicationData.AppTitle, e.Event));
                //};
            }
        }

        private static bool IsMouseButtonValidForTakingScreenshot(MouseEventArgs e)
        {
            return e.Message == EventHook.Hooks.MouseMessages.WM_LBUTTONUP || e.Message == EventHook.Hooks.MouseMessages.WM_RBUTTONUP;
        }

        private static bool IsKeyboardButtonValidForTakingScreenshot(KeyInputEventArgs e)
        {
            bool IsOk(string keyName)
            {
                return string.Compare(keyName, e.KeyData.Keyname, true) == 0;
            }

            return IsOk("return") || IsOk("tab") || IsOk("LeftCtrl");
        }

        public static void Unhook()
        {
            using (var eventHookFactory = new EventHookFactory())
            {
                keyboardWatcher?.Stop();
                mouseWatcher?.Stop();
                applicationWatcher?.Stop();
            }
        }
    }
}
