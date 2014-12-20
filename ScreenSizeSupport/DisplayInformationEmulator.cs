using ScreenSizeSupport.Misc;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScreenSizeSupport
{
  public class DisplayInformationEmulator : INotifyPropertyChanged
  {
      DisplayInformationEx displayInformationEx;
      public DisplayInformationEx DisplayInformationEx
      {
          get { return displayInformationEx; }
          set { displayInformationEx = value; }
      }

      private ScreenInfo screenInfo;
      [TypeConverter(typeof(StringToScreenInfoConverter))]
      public ScreenInfo EmulatedScreenInfo
      {
          get { return screenInfo; }
          set
          {
              screenInfo = value;
              DisplayInformationEx = new DisplayInformationEx(screenInfo.PhysicalSize, screenInfo.PhysicalResolution);
              RaisePropertyChanged();
          }
      }

    public DisplayInformationEmulator()
    {
      DisplayInformationEx = DisplayInformationEx.Default;
      DisplayInformationEx.ViewPixelsPerHostPixel = 1 + (DisplayInformationEx.ViewPixelsPerHostPixel - 1) * 0.5;
    }

    void RaisePropertyChanged([CallerMemberName] string property = "")
    {
      var handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(property));
    }

    void RaiseDisplayInformationExChanged()
    {
      var handler = DisplayInformationExChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler DisplayInformationExChanged;
  }
}
