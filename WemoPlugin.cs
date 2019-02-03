using Microsoft.Extensions.Logging;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Dynamic;
using System.Threading.Tasks;
using WemoNet;

namespace Fritz.SmartHome
{
	internal class WemoPlugin : BaseStreamDeckPlugin
	{

		// Cheer 242 cpayette 03/2/19 

		private readonly Wemo _MyWemo = new Wemo();
		private bool _State = false;
		private string _IpAddress = "";

		public ILogger Logger { get; set; }

		public override async Task OnKeyUp(StreamDeckEventPayload args)
		{

			// _MyWemo.

			if (_State) {
				Logger.LogDebug($"Turning off device at {_IpAddress}");
				await _MyWemo.TurnOffWemoPlugAsync(_IpAddress);
				// await this.Manager.SetImageAsync(args.context, "??");
			} else {
				Logger.LogDebug($"Turning on device at {_IpAddress}");
				await _MyWemo.TurnOnWemoPlugAsync(_IpAddress);
				// await this.Manager.SetImageAsync(args.context, "??");
			}

		}

		public override async Task OnWillAppear(StreamDeckEventPayload args)
		{
			this._IpAddress = args.payload.settings?.IpAddress ?? "";
			this._State = args.payload.settings?.State ?? false;
		}

		public override async Task OnWillDisappear(StreamDeckEventPayload args)
		{

			var settings = new { 
				IpAddress = _IpAddress,
				State = _State
			};

			await Manager.SetSettingsAsync(args.context, settings);
		}
	}


}
