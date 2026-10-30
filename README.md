# EQEmuPatcher

EQEmu File Comparison and Patcher

![alt tag](http://i.imgur.com/FSVgkzh.png)
---

## Starfall setup notes

**Spire hosts everything players get: the patch and this patcher.** Players'
patchers talk only to Spire, never to GitHub.

In Spire, *Starfall → Client Patch*:

- **The patch.** Spire holds every file players need. `spells_us.txt`,
  `dbstr_us.txt`, `Resources/SkillCaps.txt` and `Resources/BaseData.txt` are made
  from the database by the server's `export_client_files`. Icon sheets and item
  models come from *Icons & Models*. Everything else (`dinput8.dll`, `eqhost.txt`,
  `EQUI*.xml`, `_chr.txt`, ...) is uploaded once, to the folder it goes in.
  Each file is set to Add (live and beta), Beta (beta only), Ignore (neither) or
  Remove (deleted from players' clients if found).
- **Two channels.** **Publish to Beta** publishes what Spire holds to the beta
  channel, at `<address>beta/`. **Promote Beta to Live** then sends every player
  exactly what beta published, without the Beta-only files, after showing what
  changes. A patcher reads `<channel address>rof/filelist_rof.yml` and downloads
  only the files whose MD5 differs.
- **The patcher program, one per channel.** **Build patcher** runs this
  repository's `build.sh` on the Spire server, with the channel's public patch
  address baked in. The build is staged: **Download to try** it, then **Release**
  it. Each channel's patchers update themselves to it from
  `<channel address><name>-hash.txt` and `<channel address><name>.exe`. The beta
  patcher has its own file name (e.g. `starfallpatcher-beta.exe`). Testers run it
  in a copy of the client folder of its own: the two patchers keep their
  settings in the same `starfall.yml`, and the live patcher leaves Beta-only
  files where it finds them.
- **Its own file names.** The patcher keeps its settings in `starfall.yml` and
  the downloaded file list in `starfall_filelist.yml`, not the stock
  `eqemupatcher.yml` and `filelist.yml` every server's eqemupatcher writes. A
  client folder another server's patcher has used (Project Quarm's writes
  `clientVersion: Quarm`) no longer stops it, and a settings file it cannot read
  starts over with defaults.

To set that up, on the Spire server:

1. Install the .NET SDK where Spire runs. In akk-stack that is inside the
   `eqemu-server` container, as the eqemu user (Debian has no `dotnet-sdk`
   package of its own). Put it in the server folder, which akk-stack keeps
   outside the container, unlike the rest of the container's home folder:
   `curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0 --install-dir ~/server/.dotnet`.
   Spire looks there (the page shows the exact folder, and which dotnet it found).
2. Clone this repository into the server folder too, e.g.
   `git clone -b spire-hosted https://github.com/Zero-Hex/starfall-patcher ~/server/starfall-patcher`.
3. On the Client Patch page, set the source folder to that clone and the public
   address to how *players* reach Spire (e.g. `http://15.204.93.56:3000/api/v1/patch/`).
   Tick *git pull before building* to build what is pushed here.

`build.sh` builds the patcher anywhere the .NET SDK is installed:

```sh
FILELIST_URL=http://host:3000/api/v1/patch/ PATCHER_URL=http://host:3000/api/v1/patch/ \
SERVER_NAME="Starfall" FILE_NAME=eqemupatcher VERSION=1.1.0.1 ./build.sh out/
```

### Moving players over from GitHub (once)

Patchers built before this read their file list and update themselves from this
repository's latest release. One last release moves them to Spire. `build.yml`
makes it when this branch is merged to `master`. It builds the patcher with
Spire as both addresses, and attaches a copy of Spire's `filelist_rof.yml`,
because old patchers read that before they update themselves and stop without
one. So publish in Spire first; the build fails if Spire has nothing published.
After players have run their patcher once, GitHub releases are no longer read,
and this repository can be made private.

GitHub Actions has to be enabled for that release. `check-windows.yml` builds the
patcher on Linux and loads it under .NET Framework on Windows whenever its code
changes.

The files this repository used to host (the old `rof/` folder) are in its git
history.

---

There are two ways to set up eqemupatcher:
The easier but limited is the [Quick Guide](#quick-guide) steps.
The more custom but advanced is the [Advanced Build](#advanced-build) steps.

# Quick Guide

[![EQEmu Patcher Video](http://img.youtube.com/vi/oS2V0BABNvk/0.jpg)](http://www.youtube.com/watch?v=oS2V0BABNvk "Want Your Own EQ Patcher? Quick Guide")

click above to watch a step by step video

# Advanced Build

* eqemupatcher.exe is the client side patcher. Read Client Setup for more information on how to use it.
* filelistbuilder.exe is the server side patch prepper. Read Server Setup for more information on how to use it.

### Client Setup

After finishing the Server Setup process, simply distribute eqemupatcher.exe to your players. There are no additional dependencies for modern Windows users. They will need to place it in the same directory as eqgame.exe (Your Everquest directory).
* It is recommended to advise players that if they copy this file into Program Files for their EQ dir, it will need to run with administrator privileges.

### Server Setup

There are two parts to getting eqemupatcher working. First is getting filelistbuilder configured, second is configuring and compiling the eqemupatcher client.

* Fork this repository, so you can modify and version your own changes. If you are unfamiliar with forking, I suggest checking out https://help.github.com/articles/fork-a-repo/ to learn more. When I refer to the EQemu Patcher\ directory, I am referring to your forked copy of the source code here.
* Enable CICD actions

#### Filelistbuilder setup - Building the patch data.

Theere are two ways to generate the patch data. If you don't have large patches, you can host it on github, by copying the content to the repo's rof/ folder. You can do this on the website if no files exceed 25mb. If they exceed 25mb, I recommend using Github Desktop.

* *Optional:* If you have golang installed, you can compile filelistbuilder.go yourself by going using [this repo](https://github.com/xackery/filelistbuilder).
* Download the filelistbuilder binary version that fits your server operating system here: https://github.com/xackery/filelistbuilder/releases
* *Note:* It is important that your server generates the filelist.yml file, as an md5 can change when being hosted and cause challenges.
* Copy the filelistbuilder binary from releases to your server into a fresh directory, for now on we'll call it filelistbuilder\. Note that this should be set in a PATH directory so it can be referred to from anywhere. Or if you're lazy, copy it in the SAME directory you have any filelistbuilder.yml files.
* Make a subdirectory representing clients you plan to support, e.g. filelistbuilder\rof\ and filelistbuilder\und\
* By default, all versiontypes are not supported except rof, you can edit this during client configuration if you plan to use another client.
A list of supported types by default are: 
```
VersionTypes.Unknown, //unk
VersionTypes.Titanium, //tit
VersionTypes.Underfoot, //und
VersionTypes.Secrets_Of_Feydwer, //sof
VersionTypes.Seeds_Of_Destruction, //sod
VersionTypes.Rain_Of_Fear, //rof
VersionTypes.Rain_Of_Fear_2 //rof
VersionTypes.Broken_Mirror, //bro
```
* Copy EQEmu Patcher\filelistbuilder.yml to filelistbuilder\rof\ and any other clients.
* Copy the files that need to be patched into filelistbuilder\rof\ and any other clients.
* Edit filelistbuilder\rof\filelistbuilder.yml and change the downloadprefix line to match where the patches will be hosted. (For the example, it will be http://example.com/rof/). 
* Run the filelistbuilder binary. If all succeeds, it will generate 2 new files: filelist_rof.yml and patch.zip. (the _rof suffix will change if you modify the client)
* Patch.zip is a fully encompassed zip you can link players to who do not trust your patcher. Open it and you should see all patch contents.
* You can take a peek at filelist_rof.yml to verify the files expected to be patched are located correctly, and if the prefixdownload url looks correct.
* You can now copy all contents, except the filelistbuilder binary and filelistbuilder.yml, to your patch URL with a subdir of your client.. e.g. copy all files for a rof patch to http://example.com/rof/ so that if you access http://example.com/rof/filelist_rof.yml it will succeed to find the file
* *Note:* eqemupatcher supports deleting files by adding a delete.txt file to filelistbuilder\rof\. Inside it, simply list all files to be deleted in a different line, e.g.: (Be careful with this feature)
```
nektulos.eqg
anotherfile.txt
```
* *Note:* You can make a custom splash screen by creating a 400x450 png file named eqemupatcher.png and including it in your patcher.
* You're done! Any time you run the filelistbuilder binary in future, it will regenerate the filelist_rof.yml and patch.zip. It stores md5 checksums so that eqemupatcher.exe can compare existing files and decide to download and patch or not.

#### Visual Studio Setup - Building the Client
* Get Visual Studio if you don't have it already. You can obtain the latest copy of Visual Studio for free from Microsoft here: https://www.visualstudio.com/downloads/ Click the Community Edition.

* Navigate into the EQEmu Patcher directory and open EQEmu Patcher.sln. It will open in Visual Studio.
* On the Debug dropdown on the top center of screen, change it to Release.
* Click play. It should prompt a message box noting you must run this patcher in your Everquest directory.
* Go to your Everquest directory, and copy eqgame.exe from your EQ directory to the EQEmu Patcher\EQEmu Patcher\bin\Release directory. 
* Click play again. This time, it should detect your client and start the patcher. (By default, it is configured to use rebuildeq and may not work as desired.).
* Right click MainForm.cs on the right side of Visual Studio and click View Code.
* On the header section of this file, you will see options to configure. Change serverName, fileListUrl (set it to the ROOT URL, e.g. http://example.com/, etc to the directory you have planned to prep everything.
* In your EQEmu Patcher\EQEmu Patcher\bin\Release\ folder should be an eqemupatcher.exe file. Copy this to your server to be downloaded by clients.
* You can also verify all is ok by copying eqemupatcher.exe to your Everquest directory. 

That's it!


