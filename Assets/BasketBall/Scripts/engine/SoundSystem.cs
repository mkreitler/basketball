/*
 *  Attach this script to a game object with multiple AudioSources.
 *  The SoundSystem will assume the first Source is a music track.
 *  All other Sources will be treated as sound effects channels.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.thinkagaingames.engine {
	public class SoundSystem : PausableBehaviour {
		// Types and Constants ////////////////////////////////////////////////////
		[System.Serializable]
		private class AudioChannel {
			public AudioSource source = null;
			public int ID = -1;
			public float lastStartTime = 0f;
			public float volume = 1f;

			public AudioChannel(AudioSource source, int id, float currentTime) {
				this.source = source;
				ID = id;
				lastStartTime = currentTime;
			}
		}

		[System.Serializable]
		private class MusicInfo {
			public string track = null;
			public AudioClip file = null;
		}

		// Editor Variables ///////////////////////////////////////////////////////
		[SerializeField]
		private float musicFadeDuration = 0.5f;

		[SerializeField]
		private float newMusicDelay = 0.5f;

		[SerializeField]
		private List<MusicInfo> songData = null;

		// Interface //////////////////////////////////////////////////////////////
		// Static -----------------------------------------------------------------
		public static float SfxVolume {get; set;}
		public static float MusicVolume {get; set;}
		public static string NextMusicTrack {get; set;}

		public static void SetChannelVolume(int id, float newVolume) {
			AudioChannel channel = ChannelFromID(id);

			channel.volume = newVolume;
			channel.source.volume = newVolume * (IsMusicChannel(channel) ? MusicVolume : SfxVolume);
		}

		public static void FadeOutMusic(float duration = -1f) {
			Assert.That(Instance != null, "(FadeOutMusic) Invalid instance!");

			if (duration <= 0) {
				duration = Instance.musicFadeDuration;
			}

			Instance.StartCoroutine("FadeOutMusic");
		}

		public int PlayMusic(string songName, bool looped = true) {
			AudioClip songData = GetSong(songName);

			if (songData != null && musicChannel != null) {
				musicChannel.source.Stop();

				musicChannel.source.clip = songData;
				musicChannel.source.loop = looped;

				musicChannel.source.Play();
			}

			return musicChannel.ID;
		}

		// Instance ---------------------------------------------------------------

		// Implementation /////////////////////////////////////////////////////////
		private static SoundSystem Instance = null;
		private static List<AudioChannel> sfxChannels = new List<AudioChannel>();
		private static List<AudioChannel> allChannels = new List<AudioChannel>();
		private static Hashtable songBank = new Hashtable();
		private static AudioChannel musicChannel = null;

		// A channel's ID is a direct index into the AllChannels list, but
		// the sfxChannels has one fewer element (since the music channel
		// doesn't count as an sfx channel). Therefore, we need a utility
		// method to retrieve the correct channel from the sfx list given
		// the channel's ID.
		private static AudioChannel SfxChannelFromID(int id) {
			AudioChannel channel = id > 0 && id <= sfxChannels.Count ? sfxChannels[id - 1] : null;
			Assert.That(channel.ID == id, "Invalid sfx channel list!", Instance.gameObject);

			return channel;
		}

		private static AudioChannel ChannelFromID(int id) {
			AudioChannel channel = id >= 0 && id < allChannels.Count ? allChannels[id] : null;
			Assert.That(channel.ID == id, "Invalid 'all' channel list!", Instance.gameObject);

			return channel;
		}

		private static bool IsMusicChannel(AudioChannel channel) {
			return channel != null ? (channel.ID == 0) : false;
		}

		private static AudioClip GetSong(string songName) {
			return (AudioClip)songBank[songName];
		}

		private static void BuildChannelLists(GameObject gameObject) {
			AudioSource[] sources = gameObject.GetComponents<AudioSource>();
			for (int i=1; i<sources.Length; ++i) {
				AudioChannel channel = new AudioChannel(sources[i], i, Time.time);
				allChannels.Add(channel);
				sfxChannels.Add(channel);
			}

			if (sources.Length > 0) {
				musicChannel = new AudioChannel(sources[0], 0, Time.time);
				allChannels.Insert(0, musicChannel);
			}
		}

		private void BuildSongBank() {
			for (int i=0; i<songData.Count; ++i) {
				songBank[songData[i].track] = songData[i].file;
			}
		}

		// Interfaces /////////////////////////////////////////////////////////////
		protected override void Awake() {
			base.Awake();

			Assert.That(Instance == null, "Multiple SoundSystems in scene!", gameObject);
			Instance = this;
		}

		protected override void Start() {
			base.Start();
			
			BuildChannelLists(gameObject);
			BuildSongBank();

			MusicVolume = 1f;
			SfxVolume = 1f;
		}

		protected void OnDelete() {
			allChannels.Clear();
			sfxChannels.Clear();
			songBank.Clear();

			musicChannel = null;
			Instance = null;
		}

		// Coroutines /////////////////////////////////////////////////////////////
		private IEnumerator FadeOut() {
			Assert.That(musicChannel != null, "Invalid music channel!", Instance.gameObject);

			float timer = musicFadeDuration;
			float startVolume = musicChannel.volume;


			while (timer > 0f) {
				timer -= Time.fixedDeltaTime;
				float newVolume = startVolume * timer / musicFadeDuration;
				SetChannelVolume(musicChannel.ID, newVolume);

				yield return new WaitForFixedUpdate();
			}

			if (NextMusicTrack != null) {
				timer = newMusicDelay;

				while (timer > 0f) {
					timer -= Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}

				PlayMusic(NextMusicTrack);
				NextMusicTrack = null;
			}
		}

		// Message Handlers ///////////////////////////////////////////////////////
	}
}
