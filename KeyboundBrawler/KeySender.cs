using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeyboundBrawler;

// Posts keyboard messages to the game window, so it works even when the window is not focused.
public static class KeySender
{
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const int VkReturn = 0x0D;
    private const int VkBack = 0x08;

    [DllImport("user32.dll")]
    private static extern bool PostMessageW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    // A Chinese/Japanese/Korean IME left in its native mode eats the posted Enter (and the letters),
    // so nothing is sent until the user presses Shift to switch it to English. Close the IME while we
    // type and reopen it afterwards. Must run on the game's main thread, which owns the window.
    [DllImport("imm32.dll")]
    private static extern IntPtr ImmGetContext(IntPtr hWnd);

    [DllImport("imm32.dll")]
    private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hImc);

    [DllImport("imm32.dll")]
    private static extern bool ImmGetOpenStatus(IntPtr hImc);

    [DllImport("imm32.dll")]
    private static extern bool ImmSetOpenStatus(IntPtr hImc, bool open);

    private static bool imeClosedByUs;

    // Closes the window's IME if it is open; remembers to reopen it in RestoreIme.
    public static void SuppressIme()
    {
        var hWnd = Window;
        if (hWnd == IntPtr.Zero) return;
        var imc = ImmGetContext(hWnd);
        if (imc == IntPtr.Zero) return;
        try
        {
            if (!ImmGetOpenStatus(imc)) return;
            if (ImmSetOpenStatus(imc, false) && !imeClosedByUs)
            {
                imeClosedByUs = true;
                Plugin.Log.Info("[auto] IME was on, turned it off while typing");
            }
        }
        finally
        {
            ImmReleaseContext(hWnd, imc);
        }
    }

    // Reopens the IME if SuppressIme closed it.
    public static void RestoreIme()
    {
        if (!imeClosedByUs) return;
        imeClosedByUs = false;
        var hWnd = Window;
        if (hWnd == IntPtr.Zero) return;
        var imc = ImmGetContext(hWnd);
        if (imc == IntPtr.Zero) return;
        ImmSetOpenStatus(imc, true);
        ImmReleaseContext(hWnd, imc);
        Plugin.Log.Info("[auto] IME turned back on");
    }

    private static IntPtr window;

    private static IntPtr Window
    {
        get
        {
            if (window == IntPtr.Zero)
                window = Process.GetCurrentProcess().MainWindowHandle;
            return window;
        }
    }

    public static void Char(char c)
    {
        GameInput.MarkActive();
        SuppressIme();
        PostMessageW(Window, WmChar, c, 1);
    }

    public static void Enter() => Key(VkReturn, 0x1C);

    // The minigame box ignores a bare Backspace key press; it deletes on the backspace character.
    // Sending both may delete twice, which is harmless because we only clear whole words.
    public static void Backspace()
    {
        Key(VkBack, 0x0E);
        PostMessageW(Window, WmChar, VkBack, 1 | 0x0E << 16);
    }

    private static void Key(int vk, int scanCode)
    {
        GameInput.MarkActive();
        SuppressIme();
        PostMessageW(Window, WmKeyDown, vk, 1 | scanCode << 16);
        PostMessageW(Window, WmKeyUp, vk, unchecked((int)0xC0000001) | scanCode << 16);
    }
}
