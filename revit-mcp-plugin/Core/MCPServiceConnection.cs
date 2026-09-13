using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Media.Imaging;

namespace revit_mcp_plugin.Core
{
    [Transaction(TransactionMode.Manual)]
    public class MCPServiceConnection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 获取socket服务
                // Obtain socket service.
                SocketService service = SocketService.Instance;

                if (service.IsRunning)
                {
                    service.Stop();
                    TaskDialog.Show("revitMCP", "Close Server");
                }
                else
                {
                    service.Initialize(commandData.Application);
                    service.Start();
                    TaskDialog.Show("revitMCP", "Open Server");
                }

                UpdateButtonState(service);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private void UpdateButtonState(SocketService service)
        {
            PushButton button = Application.ToggleButton;
            if (button == null)
                return;

            if (service.IsRunning)
            {
                button.LargeImage = new BitmapImage(new Uri("/revit-mcp-plugin;component/Core/Ressources/icon-32-on.png", UriKind.RelativeOrAbsolute));
                button.ToolTip = $"MCP Server: On\r\nListening on port {service.Port}\r\nClick to stop";
            }
            else
            {
                button.LargeImage = new BitmapImage(new Uri("/revit-mcp-plugin;component/Core/Ressources/icon-32-off.png", UriKind.RelativeOrAbsolute));
                button.ToolTip = $"MCP Server: Off\r\nClick to start (port {service.Port})";
            }
        }
    }
}
