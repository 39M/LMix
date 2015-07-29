// This script can be used to control the system mouse - position of the mouse cursor and clicks
// Author: Akhmad Makhsadov
//

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class MouseControl
{
    // Import function mouse_event() from WinApi
    [DllImport("User32.dll")] 
    private static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, System.UIntPtr dwExtraInfo);

    // Flags needed to specify the mouse action 
    [System.Flags]
    private enum MouseFlags 
	{ 
        Move = 0x0001, 
        LeftDown = 0x0002, 
        LeftUp = 0x0004, 
        RightDown = 0x0008,
        RightUp = 0x0010,
        Absolute = 0x8000,
    }
                
//    public static int MouseXSpeedCoef = 45000; // Cursor rate in Х direction
//    public static int MouseYSpeedCoef = 45000; // Cursor rate in Y direction

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner
	}
	
	[DllImport("user32.dll")]
	//[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
	
	[DllImport("user32.dll")]
	static extern IntPtr GetActiveWindow();

	enum GetWindow_Cmd : uint 
	{
		GW_HWNDFIRST = 0,
		GW_HWNDLAST = 1,
		GW_HWNDNEXT = 2,
		GW_HWNDPREV = 3,
		GW_OWNER = 4,
		GW_CHILD = 5,
		GW_ENABLEDPOPUP = 6
	}
	
	[DllImport("user32.dll", SetLastError = true)]
	static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
	
//	private static int windowX = 0;
//	private static int windowY = 0;
//	private static int winSizeX = 0;
//	private static int winSizeY = 0;

	private static bool winRectPrinted = false;


    // Public function to move the mouse cursor to the specified position
    public static void MouseMove(Vector3 screenCoordinates, GUIText debugText)
    {
		int windowX = 0;
		int windowY = 0;
		int winSizeX = 0;
		int winSizeY = 0;

		bool isConvertToFullScreen = Screen.fullScreen;

		IntPtr hWnd = GetActiveWindow();
		hWnd = GetClosestWindow(hWnd, Screen.width, Screen.height);

		if(hWnd != IntPtr.Zero)
		{
			RECT winRect;

			if(GetWindowRect(hWnd, out winRect))
			{
				winSizeX = winRect.Right - winRect.Left;
				winSizeY = winRect.Bottom - winRect.Top;

				windowX = winRect.Left + (winSizeX - (int)Screen.width) / 2;
				
				if(!isConvertToFullScreen)
				{
					windowY = winRect.Top + (winSizeY - (int)Screen.height + 36) / 2;
				}
				else
				{
					windowY = winRect.Top + (winSizeY - (int)Screen.height) / 2;
				}

				if(!winRectPrinted)
				{
//					Debug.Log(string.Format("scrRes: ({0}, {1})", Screen.currentResolution.width, Screen.currentResolution.height));
//					Debug.Log(string.Format("winRect: ({0}, {1}, {2}, {3})", winRect.Left, winRect.Top, winRect.Right, winRect.Bottom));
					winRectPrinted = true;
				}
			}
		}

		int mouseX = 0;
		int mouseY = 0;

		if(!isConvertToFullScreen)
		{
			float screenX = windowX + screenCoordinates.x * Screen.width;
			float screenY = windowY + (1f - screenCoordinates.y) * Screen.height;

			float screenRelX = screenX / Screen.currentResolution.width;
			float screenRelY = screenY / Screen.currentResolution.height;
			
//			if(debugText)
//			{
//				if(!debugText.guiText.text.Contains("ScrPos"))
//				{
//					debugText.guiText.text += string.Format(", ScrSize: ({0}, {1}), WinSize: ({2}, {3}), WinPos: ({4}, {5}), ScrPos: ({6:F0}, {7:F0})", 
//					                                        Screen.width, Screen.height, 
//					                                        winSizeX, winSizeY, 
//					                                        windowX, windowY,
//					                                        screenX, screenY);
//				}
//			}

			mouseX = (int)(screenRelX * 65535);
			mouseY = (int)(screenRelY * 65535);
		}
		else
		{
			mouseX = (int)(screenCoordinates.x * 65535);
			mouseY = (int)((1f - screenCoordinates.y) * 65535);
		}

		mouse_event(MouseFlags.Absolute | MouseFlags.Move, mouseX, mouseY, 0, System.UIntPtr.Zero);
    }

	// find the closest matching child window to the screen size
	private static IntPtr GetClosestWindow(IntPtr hWndMain, int scrWidth, int scrHeight)
	{
		if(hWndMain == IntPtr.Zero)
			return hWndMain;
		
		IntPtr hWnd = hWndMain;
		RECT winRect;
		
		if(GetWindowRect(hWndMain, out winRect))
		{
			int winSizeX = winRect.Right - winRect.Left;
			int winSizeY = winRect.Bottom - winRect.Top;
			int winDiff = Math.Abs(winSizeX - scrWidth) + Math.Abs(winSizeY - scrHeight);

			IntPtr hWndChild = GetWindow(hWndMain, GetWindow_Cmd.GW_CHILD);
			int winDiffMin = winDiff;
			
			while(hWndChild != IntPtr.Zero)
			{
				if(GetWindowRect(hWndChild, out winRect))
				{
					winSizeX = winRect.Right - winRect.Left;
					winSizeY = winRect.Bottom - winRect.Top;
					winDiff = Math.Abs(winSizeX - scrWidth) + Math.Abs(winSizeY - scrHeight - 36);
					
					if(scrWidth <= winSizeX && scrHeight <= winSizeY && winDiff <= winDiffMin)
					{
						hWnd = hWndChild;
						winDiffMin = winDiff;
					}
				}
				
				hWndChild = GetWindow(hWndChild, GetWindow_Cmd.GW_HWNDNEXT);
			}
		}
		
		return hWnd;
	}
	
	// Public function to emulate a mouse button click (left button)
    public static void MouseClick()
    {
        mouse_event(MouseFlags.LeftDown, 0, 0, 0, System.UIntPtr.Zero);
        mouse_event(MouseFlags.LeftUp, 0, 0, 0, System.UIntPtr.Zero);
    }
	
    // Public function to emulate a mouse drag event (left button)
    public static void MouseDrag()
    {
        mouse_event(MouseFlags.LeftDown, 0, 0, 0, System.UIntPtr.Zero);
    }
	
    // Public function to emulate a mouse release event (left button)
    public static void MouseRelease()
    {
        mouse_event(MouseFlags.LeftUp, 0, 0, 0, System.UIntPtr.Zero);
    }
	
}


