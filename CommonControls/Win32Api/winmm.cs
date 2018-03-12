using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace CommonControls.Win32Api
{
    public static class winmm
    {
        #region Winmm.dll functions
        #region Enums
        #region Channel
        public enum Channel
        {
            Uniform = 0,
            Left = 1,
            Right = 2,
            Channel_1 = 1,
            Channel_2 = 2,
            Channel_3 = 3,
            Channel_4 = 4
        }
        #endregion

        #region MIXERLINE_LINEFLAG
        public enum MIXERLINE_LINEFLAG : uint
        {
            ACTIVE = 0x00000001,
            DISCONNECTED = 0x00008000,
            SOURCE = 0x80000000
        }
        #endregion

        #region MIXERCONTROL_CONTROLFLAG
        public enum MIXERCONTROL_CONTROLFLAG : uint
        {
            UNIFORM = 0x00000001,
            MULTIPLE = 0x00000002,
            DISABLED = 0x80000000
        }
        #endregion

        #region MIXER_GETCONTROLDETAILSFLAG
        public enum MIXER_GETCONTROLDETAILSFLAG
        {
            VALUE = 0x00000000,
            LISTTEXT = 0x00000001,
            QUERYMASK = 0x0000000F
        }
        #endregion

        #region MIXER_SETCONTROLDETAILSFLAG
        public enum MIXER_SETCONTROLDETAILSFLAG
        {
            VALUE = 0x00000000,
            CUSTOM = 0x00000001,
            QUERYMASK = 0x0000000F
        }
        #endregion

        #region MIXER_OBJECTFLAG
        public enum MIXER_OBJECTFLAG : uint
        {
            HANDLE = 0x80000000,
            MIXER = 0x00000000,
            HMIXER = (HANDLE | MIXER),
            WAVEOUT = 0x10000000,
            HWAVEOUT = (HANDLE | WAVEOUT),
            WAVEIN = 0x20000000,
            HWAVEIN = (HANDLE | WAVEIN),
            MIDIOUT = 0x30000000,
            HMIDIOUT = (HANDLE | MIDIOUT),
            MIDIIN = 0x40000000,
            HMIDIIN = (HANDLE | MIDIIN),
            AUX = 0x50000000,
        }
        #endregion

        #region MIXER_GETLINECONTROLSFLAG
        public enum MIXER_GETLINECONTROLSFLAG
        {
            ALL = 0x00000000,
            ONEBYID = 0x00000001,
            ONEBYTYPE = 0x00000002,

            QUERYMASK = 0x0000000F,
        }
        #endregion

        #region MIXER_GETLINEINFO
        public enum MIXER_GETLINEINFOF
        {
            DESTINATION = 0x00000000,
            SOURCE = 0x00000001,
            LINEID = 0x00000002,
            COMPONENTTYPE = 0x00000003,
            TARGETTYPE = 0x00000004,

            QUERYMASK = 0x0000000F
        }
        #endregion

        #region MIXERCONTROL_CT_CLASS
        public enum MIXERCONTROL_CT_CLASS : uint
        {
            MASK = 0xF0000000,
            CUSTOM = 0x00000000,
            METER = 0x10000000,
            SWITCH = 0x20000000,
            NUMBER = 0x30000000,
            SLIDER = 0x40000000,
            FADER = 0x50000000,
            TIME = 0x60000000,
            LIST = 0x70000000,

            MIXERCONTROL_CT_SUBCLASS_MASK = 0x0F000000,

            MIXERCONTROL_CT_SC_SWITCH_BOOLEAN = 0x00000000,
            MIXERCONTROL_CT_SC_SWITCH_BUTTON = 0x01000000,

            MIXERCONTROL_CT_SC_METER_POLLED = 0x00000000,

            MIXERCONTROL_CT_SC_TIME_MICROSECS = 0x00000000,
            MIXERCONTROL_CT_SC_TIME_MILLISECS = 0x01000000,

            MIXERCONTROL_CT_SC_LIST_SINGLE = 0x00000000,
            MIXERCONTROL_CT_SC_LIST_MULTIPLE = 0x01000000,

            MIXERCONTROL_CT_UNITS_MASK = 0x00FF0000,
            MIXERCONTROL_CT_UNITS_CUSTOM = 0x00000000,
            MIXERCONTROL_CT_UNITS_BOOLEAN = 0x00010000,
            MIXERCONTROL_CT_UNITS_SIGNED = 0x00020000,
            MIXERCONTROL_CT_UNITS_UNSIGNED = 0x00030000,
            MIXERCONTROL_CT_UNITS_DECIBELS = 0x00040000, /* in 10ths */
            MIXERCONTROL_CT_UNITS_PERCENT = 0x00050000, /* in 10ths */
        }
        #endregion

        #region MIXERCONTROL_CONTROLTYPE
        public enum MIXERCONTROL_CONTROLTYPE : uint
        {
            CUSTOM = (MIXERCONTROL_CT_CLASS.CUSTOM | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_CUSTOM),
            BOOLEANMETER = (MIXERCONTROL_CT_CLASS.METER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_METER_POLLED | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_BOOLEAN),
            SIGNEDMETER = (MIXERCONTROL_CT_CLASS.METER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_METER_POLLED | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_SIGNED),
            PEAKMETER = (SIGNEDMETER + 1),
            UNSIGNEDMETER = (MIXERCONTROL_CT_CLASS.METER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_METER_POLLED | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_UNSIGNED),
            BOOLEAN = (MIXERCONTROL_CT_CLASS.SWITCH | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_SWITCH_BOOLEAN | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_BOOLEAN),
            ONOFF = (BOOLEAN + 1),
            MUTE = (BOOLEAN + 2),
            MONO = (BOOLEAN + 3),
            LOUDNESS = (BOOLEAN + 4),
            STEREOENH = (BOOLEAN + 5),
            BASS_BOOST = (BOOLEAN + 0x00002277),
            BUTTON = (MIXERCONTROL_CT_CLASS.SWITCH | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_SWITCH_BUTTON | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_BOOLEAN),
            DECIBELS = (MIXERCONTROL_CT_CLASS.NUMBER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_DECIBELS),
            SIGNED = (MIXERCONTROL_CT_CLASS.NUMBER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_SIGNED),
            UNSIGNED = (MIXERCONTROL_CT_CLASS.NUMBER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_UNSIGNED),
            PERCENT = (MIXERCONTROL_CT_CLASS.NUMBER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_PERCENT),
            SLIDER = (MIXERCONTROL_CT_CLASS.SLIDER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_SIGNED),
            PAN = (SLIDER + 1),
            QSOUNDPAN = (SLIDER + 2),
            FADER = (MIXERCONTROL_CT_CLASS.FADER | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_UNSIGNED),
            VOLUME = (FADER + 1),
            BASS = (FADER + 2),
            TREBLE = (FADER + 3),
            EQUALIZER = (FADER + 4),
            SINGLESELECT = (MIXERCONTROL_CT_CLASS.LIST | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_LIST_SINGLE | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_BOOLEAN),
            MUX = (SINGLESELECT + 1),
            MULTIPLESELECT = (MIXERCONTROL_CT_CLASS.LIST | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_LIST_MULTIPLE | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_BOOLEAN),
            MIXER = (MULTIPLESELECT + 1),
            MICROTIME = (MIXERCONTROL_CT_CLASS.TIME | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_TIME_MICROSECS | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_UNSIGNED),
            MILLITIME = (MIXERCONTROL_CT_CLASS.TIME | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_SC_TIME_MILLISECS | MIXERCONTROL_CT_CLASS.MIXERCONTROL_CT_UNITS_UNSIGNED)
        }
        #endregion

        #region MIXERLINE_COMPONENTTYPE
        public enum MIXERLINE_COMPONENTTYPE
        {
            DST_FIRST = 0x00000000,
            DST_UNDEFINED = (DST_FIRST + 0),
            DST_DIGITAL = (DST_FIRST + 1),
            DST_LINE = (DST_FIRST + 2),
            DST_MONITOR = (DST_FIRST + 3),
            DST_SPEAKERS = (DST_FIRST + 4),
            DST_HEADPHONES = (DST_FIRST + 5),
            DST_TELEPHONE = (DST_FIRST + 6),
            DST_WAVEIN = (DST_FIRST + 7),
            DST_VOICEIN = (DST_FIRST + 8),
            DST_LAST = (DST_FIRST + 8),

            SRC_FIRST = 0x00001000,
            SRC_UNDEFINED = (SRC_FIRST + 0),
            SRC_DIGITAL = (SRC_FIRST + 1),
            SRC_LINE = (SRC_FIRST + 2),
            SRC_MICROPHONE = (SRC_FIRST + 3),
            SRC_SYNTHESIZER = (SRC_FIRST + 4),
            SRC_COMPACTDISC = (SRC_FIRST + 5),
            SRC_TELEPHONE = (SRC_FIRST + 6),
            SRC_PCSPEAKER = (SRC_FIRST + 7),
            SRC_WAVEOUT = (SRC_FIRST + 8),
            SRC_AUXILIARY = (SRC_FIRST + 9),
            SRC_ANALOG = (SRC_FIRST + 10),
            SRC_LAST = (SRC_FIRST + 10)
        }
        #endregion

        #region MMErrors
        public enum MMErrors
        {
            MMSYSERR_NOERROR = 0,								  /* no error */
            MMSYSERR_ERROR = (MMSYSERR_BASE + 1),  /* unspecified error */
            MMSYSERR_BADDEVICEID = (MMSYSERR_BASE + 2),  /* device ID out of range */
            MMSYSERR_NOTENABLED = (MMSYSERR_BASE + 3),  /* driver failed enable */
            MMSYSERR_ALLOCATED = (MMSYSERR_BASE + 4),  /* device already allocated */
            MMSYSERR_INVALHANDLE = (MMSYSERR_BASE + 5),  /* device handle is invalid */
            MMSYSERR_NODRIVER = (MMSYSERR_BASE + 6),  /* no device driver present */
            MMSYSERR_NOMEM = (MMSYSERR_BASE + 7),  /* memory allocation error */
            MMSYSERR_NOTSUPPORTED = (MMSYSERR_BASE + 8),  /* function isn't supported */
            MMSYSERR_BADERRNUM = (MMSYSERR_BASE + 9),  /* error value out of range */
            MMSYSERR_INVALFLAG = (MMSYSERR_BASE + 10), /* invalid flag passed */
            MMSYSERR_INVALPARAM = (MMSYSERR_BASE + 11), /* invalid parameter passed */
            MMSYSERR_HANDLEBUSY = (MMSYSERR_BASE + 12), /* handle being used */
            MMSYSERR_INVALIDALIAS = (MMSYSERR_BASE + 13), /* specified alias not found */
            MMSYSERR_BADDB = (MMSYSERR_BASE + 14), /* bad registry database */
            MMSYSERR_KEYNOTFOUND = (MMSYSERR_BASE + 15), /* registry key not found */
            MMSYSERR_READERROR = (MMSYSERR_BASE + 16), /* registry read error */
            MMSYSERR_WRITEERROR = (MMSYSERR_BASE + 17), /* registry write error */
            MMSYSERR_DELETEERROR = (MMSYSERR_BASE + 18), /* registry delete error */
            MMSYSERR_VALNOTFOUND = (MMSYSERR_BASE + 19), /* registry value not found */
            MMSYSERR_NODRIVERCB = (MMSYSERR_BASE + 20), /* driver does not call DriverCallback */
            MMSYSERR_MOREDATA = (MMSYSERR_BASE + 21), /* more data to be returned */
            MMSYSERR_LASTERROR = (MMSYSERR_BASE + 21),  /* last error in range */

            WAVERR_BADFORMAT = (WAVERR_BASE + 0),    /* unsupported wave format */
            WAVERR_STILLPLAYING = (WAVERR_BASE + 1),    /* still something playing */
            WAVERR_UNPREPARED = (WAVERR_BASE + 2),    /* header not prepared */
            WAVERR_SYNC = (WAVERR_BASE + 3),    /* device is synchronous */
            WAVERR_LASTERROR = (WAVERR_BASE + 3),    /* last error in range */

            MIXERR_INVALLINE = (MIXERR_BASE + 0),
            MIXERR_INVALCONTROL = (MIXERR_BASE + 1),
            MIXERR_INVALVALUE = (MIXERR_BASE + 2),
            MIXERR_LASTERROR = (MIXERR_BASE + 2)
        }
        #endregion
        
        public enum WAVE_FORMAT
        {
            /* WAVE form wFormatTag IDs */
            UNKNOWN = 0x0000,/* Microsoft Corporation */
            PCM = 0x0001,/* Microsoft Corporation */
            ADPCM = 0x0002,/* Microsoft Corporation */
            IEEE_FLOAT = 0x0003,/* Microsoft Corporation */
            VSELP = 0x0004,/* Compaq Computer Corp. */
            IBM_CVSD = 0x0005,/* IBM Corporation */
            ALAW = 0x0006,/* Microsoft Corporation */
            MULAW = 0x0007,/* Microsoft Corporation */
            DTS = 0x0008,/* Microsoft Corporation */
            DRM = 0x0009,/* Microsoft Corporation */
            WMAVOICE9 = 0x000A,/* Microsoft Corporation */
            WMAVOICE10 = 0x000B,/* Microsoft Corporation */
            OKI_ADPCM = 0x0010,/* OKI */
            DVI_ADPCM = 0x0011,/* Intel Corporation */
            IMA_ADPCM = DVI_ADPCM,/*  Intel Corporation */
            MEDIASPACE_ADPCM = 0x0012,/* Videologic */
            SIERRA_ADPCM = 0x0013,/* Sierra Semiconductor Corp */
            G723_ADPCM = 0x0014,/* Antex Electronics Corporation */
            DIGISTD = 0x0015,/* DSP Solutions, Inc. */
            DIGIFIX = 0x0016,/* DSP Solutions, Inc. */
            DIALOGIC_OKI_ADPCM = 0x0017,/* Dialogic Corporation */
            MEDIAVISION_ADPCM = 0x0018,/* Media Vision, Inc. */
            CU_CODEC = 0x0019,/* Hewlett-Packard Company */
            YAMAHA_ADPCM = 0x0020,/* Yamaha Corporation of America */
            SONARC = 0x0021,/* Speech Compression */
            DSPGROUP_TRUESPEECH = 0x0022,/* DSP Group, Inc */
            ECHOSC1 = 0x0023,/* Echo Speech Corporation */
            AUDIOFILE_AF36 = 0x0024,/* Virtual Music, Inc. */
            APTX = 0x0025,/* Audio Processing Technology */
            AUDIOFILE_AF10 = 0x0026,/* Virtual Music, Inc. */
            PROSODY_1612 = 0x0027,/* Aculab plc */
            LRC = 0x0028,/* Merging Technologies S.A. */
            DOLBY_AC2 = 0x0030,/* Dolby Laboratories */
            GSM610 = 0x0031,/* Microsoft Corporation */
            MSNAUDIO = 0x0032,/* Microsoft Corporation */
            ANTEX_ADPCME = 0x0033,/* Antex Electronics Corporation */
            CONTROL_RES_VQLPC = 0x0034,/* Control Resources Limited */
            DIGIREAL = 0x0035,/* DSP Solutions, Inc. */
            DIGIADPCM = 0x0036,/* DSP Solutions, Inc. */
            CONTROL_RES_CR10 = 0x0037,/* Control Resources Limited */
            NMS_VBXADPCM = 0x0038,/* Natural MicroSystems */
            CS_IMAADPCM = 0x0039,/* Crystal Semiconductor IMA ADPCM */
            ECHOSC3 = 0x003A,/* Echo Speech Corporation */
            ROCKWELL_ADPCM = 0x003B,/* Rockwell International */
            ROCKWELL_DIGITALK = 0x003C,/* Rockwell International */
            XEBEC = 0x003D,/* Xebec Multimedia Solutions Limited */
            G721_ADPCM = 0x0040,/* Antex Electronics Corporation */
            G728_CELP = 0x0041,/* Antex Electronics Corporation */
            MSG723 = 0x0042,/* Microsoft Corporation */
            MPEG = 0x0050,/* Microsoft Corporation */
            RT24 = 0x0052,/* InSoft, Inc. */
            PAC = 0x0053,/* InSoft, Inc. */
            MPEGLAYER3 = 0x0055,/* ISO/MPEG Layer3 Format Tag */
            LUCENT_G723 = 0x0059,/* Lucent Technologies */
            CIRRUS = 0x0060,/* Cirrus Logic */
            ESPCM = 0x0061,/* ESS Technology */
            VOXWARE = 0x0062,/* Voxware Inc */
            CANOPUS_ATRAC = 0x0063,/* Canopus, co., Ltd. */
            G726_ADPCM = 0x0064,/* APICOM */
            G722_ADPCM = 0x0065,/* APICOM */
            DSAT_DISPLAY = 0x0067,/* Microsoft Corporation */
            VOXWARE_BYTE_ALIGNED = 0x0069,/* Voxware Inc */
            VOXWARE_AC8 = 0x0070,/* Voxware Inc */
            VOXWARE_AC10 = 0x0071,/* Voxware Inc */
            VOXWARE_AC16 = 0x0072,/* Voxware Inc */
            VOXWARE_AC20 = 0x0073,/* Voxware Inc */
            VOXWARE_RT24 = 0x0074,/* Voxware Inc */
            VOXWARE_RT29 = 0x0075,/* Voxware Inc */
            VOXWARE_RT29HW = 0x0076,/* Voxware Inc */
            VOXWARE_VR12 = 0x0077,/* Voxware Inc */
            VOXWARE_VR18 = 0x0078,/* Voxware Inc */
            VOXWARE_TQ40 = 0x0079,/* Voxware Inc */
            SOFTSOUND = 0x0080,/* Softsound, Ltd. */
            VOXWARE_TQ60 = 0x0081,/* Voxware Inc */
            MSRT24 = 0x0082,/* Microsoft Corporation */
            G729A = 0x0083,/* AT&T Labs, Inc. */
            MVI_MVI2 = 0x0084,/* Motion Pixels */
            DF_G726 = 0x0085,/* DataFusion Systems (Pty) (Ltd) */
            DF_GSM610 = 0x0086,/* DataFusion Systems (Pty) (Ltd) */
            ISIAUDIO = 0x0088,/* Iterated Systems, Inc. */
            ONLIVE = 0x0089,/* OnLive! Technologies, Inc. */
            SBC24 = 0x0091,/* Siemens Business Communications Sys */
            DOLBY_AC3_SPDIF = 0x0092,/* Sonic Foundry */
            MEDIASONIC_G723 = 0x0093,/* MediaSonic */
            PROSODY_8KBPS = 0x0094,/* Aculab plc */
            ZYXEL_ADPCM = 0x0097,/* ZyXEL Communications, Inc. */
            PHILIPS_LPCBB = 0x0098,/* Philips Speech Processing */
            PACKED = 0x0099,/* Studer Professional Audio AG */
            MALDEN_PHONYTALK = 0x00A0,/* Malden Electronics Ltd. */
            RHETOREX_ADPCM = 0x0100,/* Rhetorex Inc. */
            IRAT = 0x0101,/* BeCubed Software Inc. */
            VIVO_G723 = 0x0111,/* Vivo Software */
            VIVO_SIREN = 0x0112,/* Vivo Software */
            DIGITAL_G723 = 0x0123,/* Digital Equipment Corporation */
            SANYO_LD_ADPCM = 0x0125,/* Sanyo Electric Co., Ltd. */
            SIPROLAB_ACEPLNET = 0x0130,/* Sipro Lab Telecom Inc. */
            SIPROLAB_ACELP4800 = 0x0131,/* Sipro Lab Telecom Inc. */
            SIPROLAB_ACELP8V3 = 0x0132,/* Sipro Lab Telecom Inc. */
            SIPROLAB_G729 = 0x0133,/* Sipro Lab Telecom Inc. */
            SIPROLAB_G729A = 0x0134,/* Sipro Lab Telecom Inc. */
            SIPROLAB_KELVIN = 0x0135,/* Sipro Lab Telecom Inc. */
            G726ADPCM = 0x0140,/* Dictaphone Corporation */
            QUALCOMM_PUREVOICE = 0x0150,/* Qualcomm, Inc. */
            QUALCOMM_HALFRATE = 0x0151,/* Qualcomm, Inc. */
            TUBGSM = 0x0155,/* Ring Zero Systems, Inc. */
            MSAUDIO1 = 0x0160,/* Microsoft Corporation */
            WMAUDIO2 = 0x0161,/* Microsoft Corporation */
            WMAUDIO3 = 0x0162,/* Microsoft Corporation */
            WMAUDIO_LOSSLESS = 0x0163,/* Microsoft Corporation */
            WMASPDIF = 0x0164,/* Microsoft Corporation */
            UNISYS_NAP_ADPCM = 0x0170,/* Unisys Corp. */
            UNISYS_NAP_ULAW = 0x0171,/* Unisys Corp. */
            UNISYS_NAP_ALAW = 0x0172,/* Unisys Corp. */
            UNISYS_NAP_16K = 0x0173,/* Unisys Corp. */
            CREATIVE_ADPCM = 0x0200,/* Creative Labs, Inc */
            CREATIVE_FASTSPEECH8 = 0x0202,/* Creative Labs, Inc */
            CREATIVE_FASTSPEECH10 = 0x0203,/* Creative Labs, Inc */
            UHER_ADPCM = 0x0210,/* UHER informatic GmbH */
            QUARTERDECK = 0x0220,/* Quarterdeck Corporation */
            ILINK_VC = 0x0230,/* I-link Worldwide */
            RAW_SPORT = 0x0240,/* Aureal Semiconductor */
            ESST_AC3 = 0x0241,/* ESS Technology, Inc. */
            GENERIC_PASSTHRU = 0x0249,
            IPI_HSX = 0x0250,/* Interactive Products, Inc. */
            IPI_RPELP = 0x0251,/* Interactive Products, Inc. */
            CS2 = 0x0260,/* Consistent Software */
            SONY_SCX = 0x0270,/* Sony Corp. */
            FM_TOWNS_SND = 0x0300,/* Fujitsu Corp. */
            BTV_DIGITAL = 0x0400,/* Brooktree Corporation */
            QDESIGN_MUSIC = 0x0450,/* QDesign Corporation */
            VME_VMPCM = 0x0680,/* AT&T Labs, Inc. */
            TPC = 0x0681,/* AT&T Labs, Inc. */
            OLIGSM = 0x1000,/* Ing C. Olivetti & C., S.p.A. */
            OLIADPCM = 0x1001,/* Ing C. Olivetti & C., S.p.A. */
            OLICELP = 0x1002,/* Ing C. Olivetti & C., S.p.A. */
            OLISBC = 0x1003,/* Ing C. Olivetti & C., S.p.A. */
            OLIOPR = 0x1004,/* Ing C. Olivetti & C., S.p.A. */
            LH_CODEC = 0x1100,/* Lernout & Hauspie */
            NORRIS = 0x1400,/* Norris Communications, Inc. */
            SOUNDSPACE_MUSICOMPRESS = 0x1500,/* AT&T Labs, Inc. */
            MPEG_ADTS_AAC = 0x1600,/* Microsoft Corporation */
            MPEG_RAW_AAC = 0x1601,/* Microsoft Corporation */
            NOKIA_MPEG_ADTS_AAC = 0x1608,/* Microsoft Corporation */
            NOKIA_MPEG_RAW_AAC = 0x1609,/* Microsoft Corporation */
            VODAFONE_MPEG_ADTS_AAC = 0x160A,/* Microsoft Corporation */
            VODAFONE_MPEG_RAW_AAC = 0x160B,/* Microsoft Corporation */
            DVM = 0x2000,/* FAST Multimedia AG */

        }

        [Flags]
        public enum SoundFlags
        {
            /// <summary>play synchronously (default)</summary>
            SND_SYNC = 0x0000,
            /// <summary>play asynchronously</summary>
            SND_ASYNC = 0x0001,
            /// <summary>silence (!default) if sound not found</summary>
            SND_NODEFAULT = 0x0002,
            /// <summary>pszSound points to a memory file</summary>
            SND_MEMORY = 0x0004,
            /// <summary>loop the sound until next sndPlaySound</summary>
            SND_LOOP = 0x0008,
            /// <summary>don't stop any currently playing sound</summary>
            SND_NOSTOP = 0x0010,
            /// <summary>Stop Playing Wave</summary>
            SND_PURGE = 0x40,
            /// <summary>don't wait if the driver is busy</summary>
            SND_NOWAIT = 0x00002000,
            /// <summary>name is a registry alias</summary>
            SND_ALIAS = 0x00010000,
            /// <summary>alias is a predefined id</summary>
            SND_ALIAS_ID = 0x00110000,
            /// <summary>name is file name</summary>
            SND_FILENAME = 0x00020000,
            /// <summary>name is resource name or atom</summary>
            SND_RESOURCE = 0x00040004
        }

        [Flags]
        public enum WaveHdrFlags : uint
        {
            WHDR_DONE = 1,
            WHDR_PREPARED = 2,
            WHDR_BEGINLOOP = 4,
            WHDR_ENDLOOP = 8,
            WHDR_INQUEUE = 16                     
        }

        public enum e_MSI_CONMMAND :uint
        { 
             MCI_OPEN                        =0x0803,
             MCI_CLOSE                       =0x0804,
             MCI_ESCAPE                      =0x0805,
             MCI_PLAY                        =0x0806,
             MCI_SEEK                        =0x0807,
             MCI_STOP                        =0x0808,
             MCI_PAUSE                       =0x0809,
             MCI_INFO                        =0x080A,
             MCI_GETDEVCAPS                  =0x080B,
             MCI_SPIN                        =0x080C,
             MCI_SET                         =0x080D,
             MCI_STEP                        =0x080E,
             MCI_RECORD                      =0x080F,
             MCI_SYSINFO                     =0x0810,
             MCI_BREAK                       =0x0811,
             MCI_SAVE                        =0x0813,
             MCI_STATUS                      =0x0814,
             MCI_CUE                         =0x0830,
             MCI_REALIZE                     =0x0840,
             MCI_WINDOW                      =0x0841,
             MCI_PUT                         =0x0842,
             MCI_WHERE                       =0x0843,
             MCI_FREEZE                      =0x0844,
             MCI_UNFREEZE                    =0x0845,
             MCI_LOAD                        =0x0850,
             MCI_CUT                         =0x0851,
             MCI_COPY                        =0x0852,
             MCI_PASTE                       =0x0853,
             MCI_UPDATE                      =0x0854,
             MCI_RESUME                      =0x0855,
             MCI_DELETE                      =0x0856
        }

        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
        public struct MIXERCONTROLDETAILS_LISTTEXT
        {
            public uint dwParam1;
            public uint dwParam2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
        public struct MIXERCONTROLDETAILS_BOOLEAN
        {
            uint fValue;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
        public struct MIXERCAPS
        {
            public short wMid;
            public short wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
            public int fdwSupport;
            public int cDestinations;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        public struct MIXERCONTROLDETAILS
        {
            [FieldOffset(0)]
            public UInt32 cbStruct;
            [FieldOffset(4)]
            public UInt32 dwControlID;
            [FieldOffset(8)]
            public UInt32 cChannels;
            // Union start
            [FieldOffset(12)]
            public IntPtr hwndOwner;
            [FieldOffset(12)]
            public UInt32 cMultipleItems;
            // Union end
            [FieldOffset(16)]
            public UInt32 cbDetails;
            [FieldOffset(20)]
            public IntPtr paDetails;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MIXERCONTROLDETAILS_UNSIGNED
        {
            public uint dwValue;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MIXERCONTROLDETAILS_SIGNED
        {
            public int value;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROL_BOUNDS
        {
            [FieldOffset(0)]
            public int lMinimum;
            [FieldOffset(4)]
            public int lMaximum;
            [FieldOffset(0)]
            public uint dwMinimum;
            [FieldOffset(4)]
            public uint dwMaximum;
            [FieldOffset(0)]
            public uint dwReserved1;
            [FieldOffset(4)]
            public uint dwReserved2;
            [FieldOffset(8)]
            public uint dwReserved3;
            [FieldOffset(12)]
            public uint dwReserved4;
            [FieldOffset(16)]
            public uint dwReserved5;
            [FieldOffset(20)]
            public uint dwReserved6;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROL_METRICS
        {
            [FieldOffset(0)]
            public uint cSteps;
            [FieldOffset(0)]
            public uint cbCustomData;
            [FieldOffset(0)]
            public uint dwReserved1;
            [FieldOffset(4)]
            public uint dwReserved2;
            [FieldOffset(8)]
            public uint dwReserved3;
            [FieldOffset(12)]
            public uint dwReserved4;
            [FieldOffset(16)]
            public uint dwReserved5;
            [FieldOffset(20)]
            public uint dwReserved6;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROL
        {
            [FieldOffset(0)]
            public UInt32 cbStruct;
            [FieldOffset(4)]
            public UInt32 dwControlID;
            [FieldOffset(8)]
            public UInt32 dwControlType;
            [FieldOffset(12)]
            public UInt32 fdwControl;
            [FieldOffset(16)]
            public UInt32 cMultipleItems;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            [FieldOffset(20)]
            public string szShortName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            [FieldOffset(20 + MIXER_SHORT_NAME_CHARS)]
            public string szName;

            [FieldOffset(20 + MIXER_SHORT_NAME_CHARS + MIXER_LONG_NAME_CHARS)]
            public MIXERCONTROL_BOUNDS Bounds;

            [FieldOffset(20 + MIXER_SHORT_NAME_CHARS + MIXER_LONG_NAME_CHARS + 24)]
            public MIXERCONTROL_METRICS Metrics;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        public struct MIXERLINECONTROLS
        {
            [FieldOffset(0)]
            public UInt32 cbStruct;
            [FieldOffset(4)]
            public UInt32 dwLineID;
            // Union start
            [FieldOffset(8)]
            public UInt32 dwControlID;
            [FieldOffset(8)]
            public MIXERCONTROL_CONTROLTYPE dwControlType;
            // Union end
            [FieldOffset(12)]
            public UInt32 cControls;
            [FieldOffset(16)]
            public UInt32 cbmxctrl;
            [FieldOffset(20)]
            public IntPtr pamxctrl;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
        public struct MIXERLINETARGET
        {
            public uint dwType;
            public uint dwDeviceID;
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
        internal struct MIXERLINE
        {
            public UInt32 cbStruct;
            public UInt32 dwDestination;
            public UInt32 dwSource;
            public UInt32 dwLineID;
            public UInt32 fdwLine;
            public IntPtr dwUser;
            public MIXERLINE_COMPONENTTYPE dwComponentType;
            public UInt32 cChannels;
            public UInt32 cConnections;
            public UInt32 cControls;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
            public MIXERLINETARGET Target;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WAVEOUTCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;

            public uint dwFormats;
            public ushort wChannels;

            public ushort wReserved1;
            public uint dwSupport;
        }

        // callbacks
        public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WAVEHDR wavhdr, int dwParam2);

        // structs        

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto,Pack = 2)]
        public struct WAVEFORMATEX
        {
            public short wFormatTag;
            public short nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public short nBlockAlign;
            public short wBitsPerSample;
            public short cbSize;

            public WAVEFORMATEX(int rate, int bits, int channels)
            {
                wFormatTag = (short)WAVE_FORMAT.PCM;
                nChannels = (short)channels;
                nSamplesPerSec = rate;
                wBitsPerSample = (short)bits;
                cbSize = 0;

                nBlockAlign = (short)(channels * (bits / 8));
                nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WAVEHDR
        {
            public IntPtr lpData; // pointer to locked data buffer
            public uint dwBufferLength; // length of data buffer
            public uint dwBytesRecorded; // used for input only
            public IntPtr dwUser; // for client's use
            public uint dwFlags; // assorted flags (see defines), WaveHdrFlags
            public uint dwLoops; // loop control counter
            public IntPtr lpNext; // PWaveHdr, reserved for driver
            public uint reserved; // reserved for driver
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MCI_PLAY_PARMS
        {
            public IntPtr dwCallback; // pointer to locked data buffer
            public uint dwFrom; // length of data buffer
            public uint dwTo; // used for input only           
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MCI_OPEN_PARMS
        {
            public IntPtr dwCallback; // pointer to locked data buffer
            public uint wDeviceID;
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string lpstrDeviceType;
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string lpstrElementName;
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string lpstrAlias;            
        }  


        #endregion

        #region Constants

        public const uint WAVE_MAPPER = unchecked((uint)(-1));

        public const int MMSYSERR_BASE = 0;
        public const int WAVERR_BASE = 32;
        public const int MIXER_SHORT_NAME_CHARS = 16;
        public const int MIXER_LONG_NAME_CHARS = 64;
        public const int MAXPNAMELEN = 32;     /* max product name length (including NULL) */
        public const int MIXERR_BASE = 1024;        
        public const int MM_MIXM_LINE_CHANGE = 0x3D0;       /* mixer line change notify */
        public const int MM_MIXM_CONTROL_CHANGE = 0x3D1;

        /* flags for dwFlags parameter in waveOutOpen() and waveInOpen() */
        public const int WAVE_FORMAT_QUERY = 0x0001;
        public const int WAVE_ALLOWSYNC = 0x0002;
        public const int WAVE_MAPPED = 0x0004;
        public const int WAVE_FORMAT_DIRECT = 0x0008;
        public const int WAVE_FORMAT_DIRECT_QUERY = (WAVE_FORMAT_QUERY | WAVE_FORMAT_DIRECT);

        /* flags used with waveOutOpen(), waveInOpen(), midiInOpen(), and */
        /* midiOutOpen() to specify the type of the dwCallback parameter. */
        public const int CALLBACK_TYPEMASK = 0x00070000;    /* callback type mask */
        public const int CALLBACK_NULL = 0x00000000;    /* no callback */
        public const int CALLBACK_WINDOW = 0x00010000;    /* dwCallback is a HWND */
        public const int CALLBACK_TASK = 0x00020000;    /* dwCallback is a HTASK */
        public const int CALLBACK_FUNCTION = 0x00030000;    /* dwCallback is a FARPROC */

        public const int CALLBACK_THREAD = (CALLBACK_TASK);/* thread ID replaces 16 bit task */
        public const int CALLBACK_EVENT = 0x00050000;    /* dwCallback is an EVENT Handle */

        /* flags for dwFlags field of WAVEHDR */
        public const int WHDR_DONE = 0x00000001; /* done bit */
        public const int WHDR_PREPARED = 0x00000002; /* set if this header has been prepared */
        public const int WHDR_BEGINLOOP = 0x00000004; /* loop start block */
        public const int WHDR_ENDLOOP = 0x00000008; /* loop end block */
        public const int WHDR_INQUEUE = 0x00000010; /* reserved for driver */


        #endregion

        [DllImport("winmm.dll")]
        public static extern int mixerOpen(out IntPtr phmx, int pMxId, IntPtr dwCallback, IntPtr dwInstance, uint fdwOpen);

        [DllImport("winmm.dll")]
        public static extern int mixerOpen(out IntPtr phmx, IntPtr pMxId, IntPtr dwCallback, IntPtr dwInstance, MIXER_OBJECTFLAG fdwOpen);

        [DllImport("winmm.dll")]
        public static extern int mixerOpen(out IntPtr phmx, IntPtr pMxId, IntPtr dwCallback, IntPtr dwInstance, uint fdwOpen);

        [DllImport("winmm.dll")]
        internal static extern int mixerGetLineInfo(IntPtr hmxobj, ref MIXERLINE pmxl, MIXER_GETLINEINFOF fdwInfo);

        [DllImport("winmm.dll")]
        public static extern int mixerClose(IntPtr hmx);

        [DllImport("winmm.dll")]
        public static extern int mixerGetLineControls(IntPtr hmxobj, ref MIXERLINECONTROLS pmxlc, MIXER_GETLINECONTROLSFLAG fdwControls);

        [DllImport("winmm.dll")]
        public static extern int mixerGetControlDetails(IntPtr hmxobj, ref MIXERCONTROLDETAILS pmxcd, MIXER_SETCONTROLDETAILSFLAG fdwDetailsmixer);

        [DllImport("winmm.dll")]
        public static extern int mixerGetControlDetails(IntPtr hmxobj, ref MIXERCONTROLDETAILS pmxcd, uint fdwDetailsmixer);

        [DllImport("winmm.dll")]
        public static extern int mixerGetControlDetails(IntPtr hmxobj, ref MIXERCONTROLDETAILS pmxcd, MIXER_GETCONTROLDETAILSFLAG fdwDetailsmixer);

        [DllImport("winmm.dll")]
        public static extern int mixerSetControlDetails(IntPtr hmxobj, ref MIXERCONTROLDETAILS pmxcd, MIXER_SETCONTROLDETAILSFLAG fdwDetails);

        [DllImport("winmm.dll")]
        public static extern int mixerGetID(IntPtr hmxobj, ref int mxId, MIXER_OBJECTFLAG fdwId);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        public static extern int mixerGetDevCaps(int uMxId, ref MIXERCAPS pmxcaps, int cbmxcaps);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern int mixerGetNumDevs();

        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint waveOutGetDevCaps(int hwo, ref WAVEOUTCAPS pwoc, uint cbwoc);

        [DllImport("winmm.dll")]
        public static extern uint waveOutGetNumDevs();
        [DllImport("winmm.dll")]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutWrite(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutOpen(out IntPtr hWaveOut, uint uDeviceID, ref WAVEFORMATEX lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);
        [DllImport("winmm.dll")]
        public static extern int waveOutOpen(out IntPtr hWaveOut, uint uDeviceID, ref WAVEFORMATEX lpFormat, int dwCallback, int dwInstance, int dwFlags);
        [DllImport("winmm.dll")]
        public static extern int waveOutReset(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutClose(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutPause(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutRestart(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

        // WaveIn calls
        [DllImport("winmm.dll")]
        public static extern int waveInGetNumDevs();
        [DllImport("winmm.dll")]
        public static extern int waveInAddBuffer(IntPtr hwi, ref WAVEHDR pwh, int cbwh);
        [DllImport("winmm.dll")]
        public static extern int waveInClose(IntPtr hwi);
        [DllImport("winmm.dll")]
        public static extern int waveInOpen(out IntPtr phwi, int uDeviceID, WAVEFORMATEX lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);
        [DllImport("winmm.dll")]
        public static extern int waveInPrepareHeader(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveInUnprepareHeader(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveInReset(IntPtr hwi);
        [DllImport("winmm.dll")]
        public static extern int waveInStart(IntPtr hwi);
        [DllImport("winmm.dll")]
        public static extern int waveInStop(IntPtr hwi);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(byte[] pszSound, IntPtr hmod, SoundFlags fdwSound);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, IntPtr hmod, SoundFlags fdwSound);

        [DllImport("winmm.dll")]
        public static extern Int32 mciSendString(String command,
           StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        [DllImport("winmm.dll")]
        public static extern Int32 mciSendCommand(UInt32 wDeviceID,
           e_MSI_CONMMAND uMsg, UInt32 fdwCommand, IntPtr dwParam);

        [DllImport("winmm.dll")]
        public static extern Int32 mciSendCommand(UInt32 wDeviceID,
           e_MSI_CONMMAND uMsg, UInt32 fdwCommand, ref MCI_OPEN_PARMS dwParam);
        [DllImport("winmm.dll")]
        public static extern Int32 mciSendCommand(UInt32 wDeviceID,
           e_MSI_CONMMAND uMsg, UInt32 fdwCommand, ref MCI_PLAY_PARMS dwParam);
        #endregion
    }
}
