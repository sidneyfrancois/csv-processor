using System.Globalization;

public class TemporaryThreadCulture : IDisposable
{
	CultureInfo _oldCulture;

	public TemporaryThreadCulture(CultureInfo newCulture)
	{
		_oldCulture = CultureInfo.CurrentCulture;
		Thread.CurrentThread.CurrentCulture = newCulture;
	}

	public void Dispose()
	{
		Thread.CurrentThread.CurrentCulture = _oldCulture;
	}
}