using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Twitch_prime_downloader
{
	public partial class FormChannelListEditor : Form
	{
		public LinkedList<string> Channels { get; private set; }

		public FormChannelListEditor(IEnumerable<string> channels)
		{
			InitializeComponent();
			textBoxChannels.Text = channels.ToText();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (textBoxChannels.Lines.Length > 0)
			{
				Channels = new LinkedList<string>();
				foreach (string t in textBoxChannels.Lines)
				{
					string channelName = t.Trim();
					if (!string.IsNullOrEmpty(channelName))
					{
						if (channelName.Contains(" "))
						{
							MessageBox.Show("Имя канала не должно содержать пробелов!", "Ошибка!",
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return;
						}

						if (Channels.Any(item => string.Equals(item, channelName, StringComparison.OrdinalIgnoreCase)))
						{
							MessageBox.Show($"Канал \"{channelName}\" добавлен более одного раза!", "Ошибка!",
								MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

						Channels.AddLast(channelName);
					}
				}

				DialogResult = DialogResult.OK;
			}

			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
