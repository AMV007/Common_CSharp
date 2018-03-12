using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Threading;


namespace CommonControls.Display
{
    public class DirectXDeviceParam
    {
        public IntPtr Control;
        public System.Drawing.Size Resolution;
        public int RefreshRate;
        public Format DisplayModeFormat;
        public bool Windowed;

        public DirectXDeviceParam()
        {
            Resolution = new System.Drawing.Size(640, 480);
            RefreshRate = 60;
            Control = IntPtr.Zero;
            DisplayModeFormat = Format.X8R8G8B8;
            Windowed = false;
        }
    }

    public class DirectXDevice
    {
        // Видеоутройство
        // VideoDevice
        Device VideoDevice = null;

        // Mutex для захвата видеоустройства
        // Videodevice get Mutex
        Mutex VideoMutex = new Mutex();

        public event EventHandler OnDeviceReset = null;
        public event EventHandler OnDeviceDisposed = null;

        DirectXDeviceParam InternalParam;

        public bool Disposed = true;
        public bool Lost = true;
        public bool Initialized = false;

        public DirectXDevice()
        {
        }

        ~DirectXDevice()
        {
            Dispose();
        }

        void onDeviceReset(object sender, EventArgs e)
        {
            if (OnDeviceReset != null) OnDeviceReset.Invoke(sender, e);
        }

        void onDeviceDisposed(object sender, EventArgs e)
        {
            if (OnDeviceDisposed != null) OnDeviceDisposed.Invoke(sender, e);
        }

        public bool Lock()
        {
            return VideoMutex.WaitOne();
        }

        public void Unlock()
        {
            // разблокируем видеоустройство
            // unlock videodevice
            VideoMutex.ReleaseMutex();
        }

        public bool CheckCooperativeLevel
        {
            get
            {
                return VideoDevice.CheckCooperativeLevel();
            }
        }

        public void Reset()
        {
            //VideoDevice.Reset(GetPresentParam(InternalParam));
            Dispose();
            InitializeDirectX(InternalParam);
        }

        public bool Exist
        {
            get
            {
                if (VideoDevice == null || Lost||Disposed || VideoDevice.Disposed || !VideoDevice.CheckCooperativeLevel()) return false;
                return true;
            }

        }

        public void BeginScene()
        {
            if (!Exist) return;
            VideoDevice.BeginScene();
        }

        public void EndScene()
        {
            if (!Exist) return;
            VideoDevice.EndScene();
        }

        public void Present()
        {
            if (!Exist) return;
            VideoDevice.Present();
        }

        public void Clear(System.Drawing.Color BackColor)
        {
            if (!Exist) return;
            VideoDevice.Clear(ClearFlags.Target, BackColor, 1.0f, 0);
        }

        PresentParameters GetPresentParam(DirectXDeviceParam Param)
        {
            PresentParameters d3dpp = new PresentParameters();

            // полноэкранное
            // full screen
            d3dpp.Windowed = Param.Windowed;

            // используем z - буффер
            // using z-buffer
            d3dpp.EnableAutoDepthStencil = true;
            d3dpp.AutoDepthStencilFormat = DepthFormat.D16;

            d3dpp.SwapEffect = SwapEffect.Discard;

            // задаем разрешение
            // setting resolution
            d3dpp.BackBufferWidth = Param.Resolution.Width;
            d3dpp.BackBufferHeight = Param.Resolution.Height;

            // формат пиклесей
            // pixel format
            d3dpp.BackBufferFormat = Format.X8R8G8B8;

            // с вертикальной синхронизацией
            // using vertical sync
            d3dpp.PresentationInterval = PresentInterval.One;

            return d3dpp;
        }

        // инициализация directX
        // DirectX initialization
        public bool InitializeDirectX(DirectXDeviceParam Param)
        {
            InternalParam = Param;
            // удаляем DirectX устройство 
            // deleteing DirectX device
            Dispose();
            Initialized = true;

            if (Lock())
            {
                try
                {
                    // Для каждого видеоадаптера, присутствующего в системе,
                    // проверяем поддерживаемые разрешения, на предмет интересующего

                    // For each adapter, examine all of its display modes to see if any 
                    // of them can give us the hardware support we desire.                

                    int adapterNumber = -1;

                    // Для каждого адаптера ...
                    // For each Adapter...
                    foreach (AdapterInformation adapter in Manager.Adapters)
                    {
                        // Проверяем каждое возможное разрешение разрешение
                        // Examine each display mode available.
                        foreach (DisplayMode display in adapter.SupportedDisplayModes)
                        {

                            // Этот адаптер поддерживает необходимое разрешение ?
                            // Does this adapter mode support needed mode ?
                            if (display.Width != Param.Resolution.Width ||
                                display.Height != Param.Resolution.Height)
                                continue;

                            // Поддерживает ли этот адаптер 32-bit RGB формат ?
                            // Does this adapter mode support a 32-bit RGB pixel format?
                            if (display.Format != Param.DisplayModeFormat)
                                continue;

                            // Поддерживает ли этот адаптер 60 Гц ? (так как у меня HDMI, то больше и не проверяю)
                            // Does this adapter mode support a refresh rate of 60 Hz?
                            if (display.RefreshRate != Param.RefreshRate)
                                continue;

                            // Мы реально его нашли !
                            // We found a match!                            
                            adapterNumber = adapter.Adapter;
                            break;
                        }
                    }

                    if (adapterNumber == -1)
                    {
                        // Не нашли, надо будет подумать что делать в этом случае
                        // TO DO: Handle lack of support for desired adapter mode...
                        throw new Exception("DirectX VideoAdapter not present");
                    }

                    //
                    // Проверяем еще раз в ручную возможности видеоадаптера
                    //
                    //
                    // Here's the manual way of verifying hardware support.
                    //

                    // У нас есть 32-bit back буффер ?
                    // Can we get a 32-bit back buffer?
                    if (!Manager.CheckDeviceType(adapterNumber,
                                                  DeviceType.Hardware,
                                                  Format.X8R8G8B8,
                                                  Format.X8R8G8B8,
                                                  false))
                    {
                        // облом, надо будет подумать позже
                        // TO DO: Handle lack of support for a 32-bit back buffer...
                        throw new Exception("32 bit back buffer not supported, adapter=" + adapterNumber.ToString() + ", default=" + Manager.Adapters.Default.Adapter.ToString());
                    }

                    // поддерживает оборудование 16 битный z - буффер
                    // Does the hardware support a 16-bit z-buffer?
                    if (!Manager.CheckDeviceFormat(adapterNumber,
                                                    DeviceType.Hardware,
                                                    Manager.Adapters[adapterNumber].CurrentDisplayMode.Format,
                                                    Usage.DepthStencil,
                                                    ResourceType.Surface,
                                                    DepthFormat.D16))
                    {
                        // возможная проблема, необходим 16-ти битный z-буффер
                        // POTENTIAL PROBLEM: We need at least a 16-bit z-buffer!
                        throw new Exception("16 bit Z buffer not supported, adapter=" + adapterNumber.ToString());
                    }

                    //
                    // Поддежрживает ли устройство аппаратную обработку вертексов,
                    // если так, то используем ее, иначе будем использовать программную
                    //

                    //
                    // Do we support hardware vertex processing? if so, use it. 
                    // If not, downgrade to software.
                    //

                    Caps caps = Manager.GetDeviceCaps(adapterNumber,
                                                       DeviceType.Hardware);
                    CreateFlags flags;

                    if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                        flags = CreateFlags.HardwareVertexProcessing;
                    else
                        flags = CreateFlags.SoftwareVertexProcessing;

                    //
                    // Все проверенно, создаем простое полноэкранное DirectX устройство
                    //

                    //
                    // Everything checks out - create a simple, full-screen device.
                    //                   

                    VideoDevice = new Device(adapterNumber, DeviceType.Hardware, Param.Control, flags, GetPresentParam(Param));
                    if (VideoDevice==null) throw new Exception("Error create Videodevice");

                    // добавляем event-ы для обработки состояния видеоустройства
                    // events for check videodevice state
                    VideoDevice.DeviceReset += new EventHandler(VideoDevice_DeviceReset);
                    VideoDevice.DeviceLost += new EventHandler(VideoDevice_DeviceLost);                    
                    Disposed = false;
                    Lost = false;
                   
                }
                catch (Exception ex)
                {
                    c_ErrorDataWork.Instance.ErrorProcess(ex);
                    Dispose();
                    Initialized = false;
                }
                finally
                {
                    // разблокируем видеоустройство
                    // deblock videodevice
                    Unlock();

                    if (Initialized)
                    {
                        // обресечиваем видеоустройство
                        // resetting videodevice
                        VideoDevice_DeviceReset(VideoDevice, null);
                    }
                }
            }
            return Initialized;
        }

        void VideoDevice_DeviceLost(object sender, EventArgs e)
        {
            // удаляем видеоустройство
            // deleting videodevice
            Dispose();
            //Lost = true;
            
        }

        void VideoDevice_DeviceReset(object sender, EventArgs e)
        {
            if (!Exist)
            {
                InitializeDirectX(InternalParam);    
            }

            if (Lock())
            {
                try
                {
                    onDeviceReset(sender, e);
                }
                catch (Exception ex)
                {
                    c_ErrorDataWork.Instance.ErrorProcess(ex);
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public void Dispose()
        {
            if (Disposed) return;

            if (Lock())
            {
                try
                {
                    // если видеоутсройство не заблокировано - удаляем параметры для работы с ним
                    // if videodevice not locked, deleting parameters for videodevice

                    VideoDevice.DeviceLost -= VideoDevice_DeviceLost;
                    VideoDevice.DeviceReset -= VideoDevice_DeviceReset;

                    onDeviceDisposed(VideoDevice, EventArgs.Empty);
                    VideoDevice.Dispose();                                        
                    Disposed = true;
                    Lost = true;
                }
                catch (Exception ex)
                {
                    c_ErrorDataWork.Instance.ErrorProcess(ex);
                }
                finally
                {
                    Unlock();
                }
            }
        }

    }
}
