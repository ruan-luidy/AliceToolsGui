using System.Text;

namespace AliceToolsGui.Wpf.Services;

public class EncodingService : IEncodingService
{
  private Encoding _defaultEncoding = Encoding.UTF8;

  public List<Encoding> GetAvailableEncodings()
  {
    return new List<Encoding>
        {
            Encoding.UTF8,
            Encoding.Unicode,
            Encoding.ASCII,
            // Encoding.GetEncoding("Shift-JIS"),
            Encoding.GetEncoding("ISO-8859-1"),
            // Encoding.GetEncoding("Windows-1252")
        };
  }

  public Encoding GetDefaultEncoding()
  {
    return _defaultEncoding;
  }

  public void SetDefaultEncoding(Encoding encoding)
  {
    _defaultEncoding = encoding;
  }
}
