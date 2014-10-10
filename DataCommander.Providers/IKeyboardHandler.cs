namespace DataCommander
{
	using System.Windows.Forms;

	public interface IKeyboardHandler
	{
		bool HandleKeyDown( KeyEventArgs e );

		bool HandleKeyPress( KeyPressEventArgs e );
	}
}