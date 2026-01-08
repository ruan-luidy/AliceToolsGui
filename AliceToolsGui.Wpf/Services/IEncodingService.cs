using System.Text;

namespace AliceToolsGui.Wpf.Services;

public interface IEncodingService
{
  List<Encoding> GetAvailableEncodings();
  Encoding GetDefaultEncoding();
  void SetDefaultEncoding(Encoding encoding);
}
