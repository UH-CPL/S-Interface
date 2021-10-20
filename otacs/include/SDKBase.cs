using System;

namespace IACFSDK
{
	/// <summary>
	/// Summary description for SDKBase.
	/// </summary>
	public class SDKBase
	{
		/// <summary>
		/// Version of a standard SDK component.
		/// </summary>
		public class Version
		{
			/// <summary>Major version.</summary>
			public ushort		Major;
			/// <summary>Minor version (features).</summary>
			public ushort		Minor;
			/// <summary>Revision (bug fixes).</summary>
			public ushort		Revision;
			/// <summary>Build (# times compiled).</summary>
			public ushort		Build;

			/// <summary>
			/// Converts the version class to a string.
			/// </summary>
			/// <returns>String representation of the version M.m.r Build: b.</returns>
			public override string ToString()
			{
				return Major.ToString() + "." + Minor.ToString() + "." + Revision.ToString() + " Build: " + Build.ToString();
			}
		}

		/// <summary>
		/// Error codes returned by the SDK.
		/// </summary>
		public enum sdkError
		{
			/// <summary>Generic failure condition.</summary>
			errFailed						= -1,
			/// <summary>The operation was successfull.</summary>
			errOk							= 0x0000,
			/// <summary>Message rejected by the device.</summary>
			errRejected						= 0x0001,
			/// <summary>Feature disabled in the device.</summary>
			errDisabled						= 0x0002,
			/// <summary>Message is corrupt (CRC/checksum error).</summary>
			errMessageCorrupt				= 0x0003,
			/// <summary>Timeout expired.</summary>
			errTimeout						= 0x0004,
			/// <summary>Device recieved message, but could not proccess.</summary>
			errRecievedUnableToProcess		= 0x0005,
			/// <summary>Write failed.</summary>
			errWriteFailed					= 0x0006,
			/// <summary>Read failed.</summary>
			errReadFailed					= 0x0007,
			/// <summary>Message is too big.</summary>
			errMessageTooBig				= 0x0008,
			/// <summary>Command or address is invalid.</summary>
			errBadAddrOrID					= 0x0009,
			/// <summary>Device returned data that is invalid.</summary>
			errBadReturnedData				= 0x000A,
			/// <summary>Message length is incorrect.</summary>
			errBadMessageLength				= 0x000C,
			/// <summary>Timeout expired.</summary>
			errMessageTimeout				= 0x000D,

			/// <summary>UnsupportedCommandData</summary>
			errUnsupportedCommandData		= 0x0040,
			/// <summary>FPAClocksOn</summary>
			errFPAClocksOn					= 0x0041,
			/// <summary>FunctionNotSupported</summary>
			errFunctionNotSupported			= 0x0042,
			/// <summary>NoMasterClockPresent</summary>
			errNoMasterClockPresent			= 0x0043,

			/// <summary>OutOfRAM</summary>
			errOutOfRAM						= 0x0050,
			/// <summary>MemoryAllocation</summary>
			errMemoryAllocation				= 0x0051,
			/// <summary>NUCInProgress</summary>
			errNUCInProgress				= 0x0052,
			/// <summary>NUCTypeNotSupported</summary>
			errNUCTypeNotSupported			= 0x0053,
			/// <summary>NUCNotEnoughSpace</summary>
			errNUCNotEnoughSpace			= 0x0054,
			/// <summary>NUCCommandNotEnabled</summary>
			errNUCCommandNotEnabled			= 0x0055,
			/// <summary>AGCAlgorithmNotSupported</summary>
			errAGCAlgorithmNotSupported		= 0x0056,
			/// <summary>NULLPointer</summary>
			errNULLPointer					= 0x0057,
			/// <summary>LoadedNUCMemoryAllocation</summary>
			errLoadedNUCMemoryAllocation	= 0x0058,
			/// <summary>RingBufferFull</summary>
			errRingBufferFull				= 0x0059,
			/// <summary>ControlNotPassedToBootLoader</summary>
			errControlNotPassedToBootLoader	= 0x005A,

			/// <summary>BlockSizeExceeded</summary>
			errBlockSizeExceeded			= 0x0070,
			/// <summary>OutOfSpace</summary>
			errOutOfSpace					= 0x0071,

			/// <summary>SPISemaphoreUnavailable</summary>
			errSPISemaphoreUnavailable		= 0x0080,
			/// <summary>FlashWriteTimeout</summary>
			errFlashWriteTimeout			= 0x0081,
			/// <summary>FlashEraseTimeout</summary>
			errFlashEraseTimeout			= 0x0082,
			/// <summary>FlashBlankCheckFail</summary>
			errFlashBlankCheckFail			= 0x0083,
			/// <summary>DivideByZero</summary>
			errDivideByZero					= 0x0084,
			/// <summary>FlashInternalEraseError</summary>
			errFlashInternalEraseError		= 0x0085,
			/// <summary>FlashVerifyWriteFailed</summary>
			errFlashVerifyWriteFailed		= 0x0086,
			/// <summary>FlashEraseSuspend</summary>
			errFlashEraseSuspend			= 0x0087,
			/// <summary>ProgramSuspend</summary>
			errProgramSuspend				= 0x0088,
			/// <summary>ProgramFailure</summary>
			errProgramFailure				= 0x0089,
			/// <summary>FlashEraseFailure</summary>
			errFlashEraseFailure			= 0x008A,
			/// <summary>FlashVPPRange</summary>
			errFlashVPPRange				= 0x008B,
			/// <summary>FlashDEVProtect</summary>
			errFlashDEVProtect				= 0x008C,
			/// <summary>FIFOWriteTimeout</summary>
			errFIFOWriteTimeout				= 0x008D,
			/// <summary>FIFOFull</summary>
			errFIFOFull						= 0x008E,
			/// <summary>FIFOEmpty</summary>
			errFIFOEmpty					= 0x008F,

			/// <summary>ItemNotFound</summary>
			errItemNotFound					= 0x0100,
			/// <summary>MsgQReceiveSync</summary>
			errMsgQReceiveSync				= 0x0101,
			/// <summary>SemaphoreTimeout</summary>
			errSemaphoreTimeout				= 0x0102,
			/// <summary>FileOpenFailed</summary>
			errFileOpenFailed				= 0x0103,
			/// <summary>OutOfDiskSpace</summary>
			errOutOfDiskSpace				= 0x0104,
			/// <summary>StatFSFailed</summary>
			errStatFSFailed					= 0x0105,
			/// <summary>FileUploadBlockNumber</summary>
			errFileUploadBlockNumber		= 0x0106,
			/// <summary>UnableToOpenDirectory</summary>
			errUnableToOpenDirectory		= 0x0107,
			/// <summary>UnableToCloseDirectory</summary>
			errUnableToCloseDirectory		= 0x0108,
			/// <summary>Filesystem directory has been closed.</summary>
			errDirectoryClosed				= 0x0109,

			/// <summary>HALCapturingData</summary>
			errHALCapturingData				= 0x010A,
			/// <summary>HALCaptureROIOutOfRange</summary>
			errHALCaptureROIOutOfRange		= 0x010B,
			/// <summary>OutOfRangeValue</summary>
			errOutOfRangeValue				= 0x010C,
			/// <summary>FileVersionInvalid</summary>
			errFileVersionInvalid			= 0x010D,

			/// <summary>Given buffer is too small to hold results.</summary>
			errBufferTooSmall				= 0x010E,

			/// <summary>DataCRCError</summary>
			errDataCRCError					= 0x010F,
			/// <summary>MaxItemsExceeded</summary>
			errMaxItemsExceeded				= 0x0110,

			/// <summary>The command or parameter id is not implemented in the camera.</summary>
			errNotImplemented				= 0x0111,

			/// <summary>BuiltInTestFailed</summary>
			errBuiltInTestFailed			= 0x0112,
			/// <summary>FileReadWrongByteNum</summary>
			errFileReadWrongByteNum			= 0x0113,
			/// <summary>FileWriteWrongByteNum</summary>
			errFileWriteWrongByteNum		= 0x0114,
			/// <summary>StringTooLong</summary>
			errStringTooLong				= 0x0115,
			/// <summary>FunctionDisabled</summary>
			errFunctionDisabled				= 0x0116,
			/// <summary>FileDoesNotExist</summary>
			errFileDoesNotExist				= 0x0117,
			/// <summary>UnsupportedFPAType</summary>
			errUnsupportedFPAType			= 0x0118,
			/// <summary>MessageQFull</summary>
			errMessageQFull					= 0x0119,
			/// <summary>NoDigitizerMode</summary>
			errNoDigitizerMode				= 0x011A,
			/// <summary>SDRAMTestFailed</summary>
			errSDRAMTestFailed				= 0x011B,
			/// <summary>FPGATestRegWriteFailed</summary>
			errFPGATestRegWriteFailed		= 0x011C,
			/// <summary>FPGATestRegReadFailed</summary>
			errFPGATestRegReadFailed		= 0x011D,
			/// <summary>XBUSWriteFailed</summary>
			errXBUSWriteFailed				= 0x011E,
			/// <summary>XBUSReadFailed</summary>
			errXBUSReadFailed				= 0x011F,
			/// <summary>XBUSTransferNotAllowed</summary>
			errXBUSTransferNotAllowed		= 0x0120,
			/// <summary>BITHistCaptureFailed</summary>
			errBITHistCaptureFailed			= 0x0121,
			/// <summary>BITNUCSDRAMFailed</summary>
			errBITNUCSDRAMFailed			= 0x0122,
			/// <summary>NUCSDRAMBusyTimeout</summary>
			errNUCSDRAMBusyTimeout			= 0x0123,
			/// <summary>BITNUCFlashFailed</summary>
			errBITNUCFlashFailed			= 0x0124,
			/// <summary>BITFrameGrabRAMFailed</summary>
			errBITFrameGrabRAMFailed		= 0x0125,
			/// <summary>BITFPGAControlRegFailed</summary>
			errBITFPGAControlRegFailed		= 0x0126,
			/// <summary>BITFileSystemFailed</summary>
			errBITFileSystemFailed			= 0x0127,
			/// <summary>BITVideoFPGAControlRegFailed</summary>
			errBITVideoFPGAControlRegFailed	= 0x0128,
			/// <summary>BITAGCLUT1Failed</summary>
			errBITAGCLUT1Failed				= 0x0129,
			/// <summary>BITAGCLUT2Failed</summary>
			errBITAGCLUT2Failed				= 0x0130,
			/// <summary>BITVideoFrameBufferFailed</summary>
			errBITVideoFrameBufferFailed	= 0x0131,
			/// <summary>WriteToFileFailed</summary>
			errWriteToFileFailed			= 0x0132,
			/// <summary>ReadFromFileFailed</summary>
			errReadFromFileFailed			= 0x0133,
			/// <summary>FileIsNotOpen</summary>
			errFileIsNotOpen				= 0x0134,
			/// <summary>FileHandleInvalid</summary>
			errFileHandleInvalid			= 0x0135,
			/// <summary>FileInvalidOperation</summary>
			errFileInvalidOperation			= 0x0136,
			/// <summary>FileInvalidBlockInChain</summary>
			errFileInvalidBlockInChain		= 0x0137,
			/// <summary>FrameGrabBusyTimeout</summary>
			errFrameGrabBusyTimeout			= 0x0138,
			/// <summary>FPGAConfigDoneTimeout</summary>
			errFPGAConfigDoneTimeout		= 0x0139,
			/// <summary>FileIsOpen</summary>
			errFileIsOpen					= 0x013A,
			/// <summary>FileIsProtected</summary>
			errFileIsProtected				= 0x013B,
			/// <summary>MessageQTransmitFail</summary>
			errMessageQTransmitFail			= 0x013C,
			/// <summary>FileNameInvalid</summary>
			errFileNameInvalid				= 0x013D,
			/// <summary>EndOfFileReached</summary>
			errEndOfFileReached				= 0x013E,
			/// <summary>SemaphoreNotCreated</summary>
			errSemaphoreNotCreated			= 0x013F,

			/// <summary>CacheEntryNotAvailable</summary>
			errCacheEntryNotAvailable		= 0x0140,
			/// <summary>FlashNotWritten</summary>
			errFlashNotWritten				= 0x0141,
			/// <summary>BuiltInTestNotFound</summary>
			errBuiltInTestNotFound			= 0x0142,

			/// <summary>FactoryCalSensorFail</summary>
			errFactoryCalSensorFail			= 0x0180,
			/// <summary>FactoryCalEdgeNotFound</summary>
			errFactoryCalEdgeNotFound		= 0x0181,
			/// <summary>FindHomeSensorFail</summary>
			errFindHomeSensorFail			= 0x0182,
			/// <summary>FindHomeEdgeNotFound</summary>
			errFindHomeEdgeNotFound			= 0x0183,
			/// <summary>PositionMoveBeforeHomeFound</summary>
			errPositionMoveBeforeHomeFound	= 0x0184,

			/// <summary>ThermistorRatioOutOfRange</summary>
			errThermistorRatioOutOfRange	= 0x0190,
			/// <summary>TECOnVolgateTooHigh</summary>
			errTECOnVolgateTooHigh			= 0x0191,
			/// <summary>TECOffVolgateTooHigh</summary>
			errTECOffVolgateTooHigh			= 0x0192,

			/// <summary>FPANotInDatabase</summary>
			errFPANotInDatabase				= 0x0200,
			/// <summary>MaskIndexTooBig</summary>
			errMaskIndexTooBig				= 0x0201,
			/// <summary>MaskUnsupported</summary>
			errMaskUnsupported				= 0x0202,
			/// <summary>BadPixelReplaceFailed</summary>
			errBadPixelReplaceFailed		= 0x0203,
			/// <summary>TooManyBadPixels</summary>
			errTooManyBadPixels				= 0x0204,

			/// <summary>TableEnterPointNotFound</summary>
			errTableEnterPointNotFound		= 0x0210,
			/// <summary>ForwardTransformAttempt</summary>
			errForwardTransformAttempt		= 0x0211,
			/// <summary>CaptureRetryTimeout</summary>
			errCaptureRetryTimeout			= 0x0212,
			/// <summary>TISAProtocolChecksum</summary>
			errTISAProtocolChecksum			= 0x0213,

			/// <summary>CCDriverAlreadyOpen</summary>
			errCCDriverAlreadyOpen			= 0x0214,
			/// <summary>CCPastEndOfCounter</summary>
			errCCPastEndOfCounter			= 0x0215,
			/// <summary>ProtectedFlashArea</summary>
			errProtectedFlashArea			= 0x0216,

			/// <summary>INIFileRead</summary>
			errINIFileRead					= 0x0217,

			/// <summary>DataSizeBad</summary>
			errDataSizeBad					= 0x0218,
			/// <summary>InvalidModuleID</summary>
			errInvalidModuleID				= 0x0219,
			/// <summary>UnsupportedCommandID</summary>
			errUnsupportedCommandID			= 0x0220,
			/// <summary>UnsupportedGetID</summary>
			errUnsupportedGetID				= 0x0221,
			/// <summary>UnsupportedSetID</summary>
			errUnsupportedSetID				= 0x0222,
			/// <summary>InvalidAccessCode</summary>
			errInvalidAccessCode			= 0x0223,
			/// <summary>InvalidVideoTapPoint</summary>
			errInvalidVideoTapPoint			= 0x0224,
			/// <summary>NUCCoeffChecksumFail</summary>
			errNUCCoeffChecksumFail			= 0x0225,
			/// <summary>LineEndNotFound</summary>
			errLineEndNotFound				= 0x0226,
			/// <summary>WriteToHwFailed</summary>
			errWriteToHwFailed				= 0x0227,
			/// <summary>ReadFromHwFailed</summary>
			errReadFromHwFailed				= 0x0228,
			/// <summary>I2CStartFailure</summary>
			errI2CStartFailure				= 0x0229,
			/// <summary>I2CRepeatStartFailure</summary>
			errI2CRepeatStartFailure		= 0x0230,	//	someone can't count in hex :-P
			/// <summary>I2CReadFailure</summary>
			errI2CReadFailure				= 0x0231,
			/// <summary>I2CWriteFailure</summary>
			errI2CWriteFailure				= 0x0232,
			/// <summary>I2CReadAddrFailure</summary>
			errI2CReadAddrFailure			= 0x0233,
			/// <summary>I2CWriteAddrFailure</summary>
			errI2CWriteAddrFailure			= 0x0234,
			/// <summary>InvalidFlagCommand</summary>
			errInvalidFlagCommand			= 0x0235,
			/// <summary>InvalidFlagState</summary>
			errInvalidFlagState				= 0x0236,
			/// <summary>InvalidFlagStepoint</summary>
			errInvalidFlagStepoint			= 0x0237,
			/// <summary>InvalidNUCFileType</summary>
			errInvalidNUCFileType			= 0x0238,
			/// <summary>IntegrationTimeTooLong</summary>
			errIntegrationTimeTooLong		= 0x0239,
			/// <summary>FramePeriodTooShort</summary>
			errFramePeriodTooShort			= 0x023A,
			/// <summary>ConfigSaveFailed</summary>
			errConfigSaveFailed				= 0x023B,
			/// <summary>ConfigRestoreFailed</summary>
			errConfigRestoreFailed			= 0x023C,
			/// <summary>InvalidAGCFileType</summary>
			errInvalidAGCFileType			= 0x023D,
			/// <summary>ReadOnlyParameter</summary>
			errReadOnlyParameter			= 0x023E,
			/// <summary>WriteOnlyParameter</summary>
			errWriteOnlyParameter			= 0x023F,
			/// <summary>FileBlockSizeZero</summary>
			errFileBlockSizeZero			= 0x0240,
			/// <summary>TransferBlockSizeTooLarge</summary>
			errTransferBlockSizeTooLarge	= 0x0241,
			/// <summary>FileNameNull</summary>
			errFileNameNull					= 0x0242,
			/// <summary>FileSizeZero</summary>
			errFileSizeZero					= 0x0243,
			/// <summary>FileInvalidTransferMode</summary>
			errFileInvalidTransferMode		= 0x0244,
			/// <summary>DirListNotComplete</summary>
			errDirListNotComplete			= 0x0245,
			/// <summary>DirNameTooLong</summary>
			errDirNameTooLong				= 0x0246,
			/// <summary>DirNameTooShort</summary>
			errDirNameTooShort				= 0x0247,
			/// <summary>DirInvalidBlockCount</summary>
			errDirInvalidBlockCount			= 0x0248,
			/// <summary>DirInvalidDriveName</summary>
			errDirInvalidDriveName			= 0x0249,
			/// <summary>CommandSetFailure</summary>
			errCommandSetFailure			= 0x0250,
			/// <summary>InvalidHistogramTap</summary>
			errInvalidHistogramTap			= 0x0251,
		}

		/// <summary>
		/// Exception class that includes an SDK error code.
		/// </summary>
		public class SDKException : ApplicationException
		{
			private int		error;

			/// <summary>
			/// Error code associated with this exception.
			/// </summary>
			public int Error
			{
				get
				{
					return error;
				}
			}

			/// <summary>
			/// Constructor, creates exception from error code.
			/// </summary>
			/// <param name="_error">error code</param>
			public SDKException(int _error) : base(SDKBase.errorString(_error))
			{
				error = _error;
			}

			/// <summary>
			/// Constructor, creates exception from error code and message.
			/// </summary>
			/// <param name="msg">message</param>
			/// <param name="_error">error code</param>
			public SDKException(string msg, int _error) : base(msg)
			{
				error = _error;
			}
		}

		/// <summary>
		/// Converts an error code to a string for display.
		/// </summary>
		/// <param name="Error">error code</param>
		/// <returns>string representation</returns>
		public static string errorString(int Error)
		{
			switch ((sdkError)Error)
			{
			case sdkError.errFailed:
				return "Unknown Failure";
			case sdkError.errOk:
				return "The operation was successfull";
			case sdkError.errRejected:
				return "Message rejected by the device";
			case sdkError.errDisabled:
				return "Feature disabled in the device";
			case sdkError.errMessageCorrupt:
				return "Message is corrupt (CRC/checksum error)";
			case sdkError.errTimeout:
				return "Timeout expired";
			case sdkError.errRecievedUnableToProcess:
				return "Device recieved message, but could not proccess";
			case sdkError.errWriteFailed:
				return "Write Failed";
			case sdkError.errReadFailed:
				return "Read Failed";
			case sdkError.errMessageTooBig:
				return "Message is too big";
			case sdkError.errBadAddrOrID:
				return "Command or address is invalid";
			case sdkError.errBadReturnedData:
				return "Device returned data that is invalid";
			case sdkError.errBadMessageLength:
				return "Message length is incorrect";
			case sdkError.errMessageTimeout:
				return "Timeout expired";

			case sdkError.errUnsupportedCommandData:
				return "UnsupportedCommandData";
			case sdkError.errFPAClocksOn:
				return "FPAClocksOn";
			case sdkError.errFunctionNotSupported:
				return "FunctionNotSupported";
			case sdkError.errNoMasterClockPresent:
				return "NoMasterClockPresent";

			case sdkError.errOutOfRAM:
				return "OutOfRAM";
			case sdkError.errMemoryAllocation:
				return "MemoryAllocation";
			case sdkError.errNUCInProgress:
				return "NUCInProgress";
			case sdkError.errNUCTypeNotSupported:
				return "NUCTypeNotSupported";
			case sdkError.errNUCNotEnoughSpace:
				return "NUCNotEnoughSpace";
			case sdkError.errNUCCommandNotEnabled:
				return "NUCCommandNotEnabled";
			case sdkError.errAGCAlgorithmNotSupported:
				return "AGCAlgorithmNotSupported";
			case sdkError.errNULLPointer:
				return "NULLPointer";
			case sdkError.errLoadedNUCMemoryAllocation:
				return "LoadedNUCMemoryAllocation";
			case sdkError.errRingBufferFull:
				return "RingBufferFull";
			case sdkError.errControlNotPassedToBootLoader:
				return "ControlNotPassedToBootLoader";

			case sdkError.errBlockSizeExceeded:
				return "BlockSizeExceeded";
			case sdkError.errOutOfSpace:
				return "OutOfSpace";

			case sdkError.errSPISemaphoreUnavailable:
				return "SPISemaphoreUnavailable";
			case sdkError.errFlashWriteTimeout:
				return "FlashWriteTimeout";
			case sdkError.errFlashEraseTimeout:
				return "FlashEraseTimeout";
			case sdkError.errFlashBlankCheckFail:
				return "FlashBlankCheckFail";
			case sdkError.errDivideByZero:
				return "DivideByZero";
			case sdkError.errFlashInternalEraseError:
				return "FlashInternalEraseError";
			case sdkError.errFlashVerifyWriteFailed:
				return "FlashVerifyWriteFailed";
			case sdkError.errFlashEraseSuspend:
				return "FlashEraseSuspend";
			case sdkError.errProgramSuspend:
				return "ProgramSuspend";
			case sdkError.errProgramFailure:
				return "ProgramFailure";
			case sdkError.errFlashEraseFailure:
				return "FlashEraseFailure";
			case sdkError.errFlashVPPRange:
				return "FlashVPPRange";
			case sdkError.errFlashDEVProtect:
				return "FlashDEVProtect";
			case sdkError.errFIFOWriteTimeout:
				return "FIFOWriteTimeout";
			case sdkError.errFIFOFull:
				return "FIFOFull";
			case sdkError.errFIFOEmpty:
				return "FIFOEmpty";

			case sdkError.errItemNotFound:
				return "ItemNotFound";
			case sdkError.errMsgQReceiveSync:
				return "MsgQReceiveSync";
			case sdkError.errSemaphoreTimeout:
				return "SemaphoreTimeout";
			case sdkError.errFileOpenFailed:
				return "FileOpenFailed";
			case sdkError.errOutOfDiskSpace:
				return "OutOfDiskSpace";
			case sdkError.errStatFSFailed:
				return "StatFSFailed";
			case sdkError.errFileUploadBlockNumber:
				return "FileUploadBlockNumber";
			case sdkError.errUnableToOpenDirectory:
				return "UnableToOpenDirectory";
			case sdkError.errUnableToCloseDirectory:
				return "UnableToCloseDirectory";
			case sdkError.errDirectoryClosed:
				return "Filesystem directory has been closed";

			case sdkError.errHALCapturingData:
				return "HALCapturingData";
			case sdkError.errHALCaptureROIOutOfRange:
				return "HALCaptureROIOutOfRange";
			case sdkError.errOutOfRangeValue:
				return "OutOfRangeValue";
			case sdkError.errFileVersionInvalid:
				return "FileVersionInvalid";

			case sdkError.errBufferTooSmall:
				return "Given buffer is too small to hold results";

			case sdkError.errDataCRCError:
				return "DataCRCError";
			case sdkError.errMaxItemsExceeded:
				return "MaxItemsExceeded";

			case sdkError.errNotImplemented:
				return "The operation is not implemented in the camera";

			case sdkError.errBuiltInTestFailed:
				return "BuiltInTestFailed";
			case sdkError.errFileReadWrongByteNum:
				return "FileReadWrongByteNum";
			case sdkError.errFileWriteWrongByteNum:
				return "FileWriteWrongByteNum";
			case sdkError.errStringTooLong:
				return "StringTooLong";
			case sdkError.errFunctionDisabled:
				return "FunctionDisabled";
			case sdkError.errFileDoesNotExist:
				return "FileDoesNotExist";
			case sdkError.errUnsupportedFPAType:
				return "UnsupportedFPAType";
			case sdkError.errMessageQFull:
				return "MessageQFull";
			case sdkError.errNoDigitizerMode:
				return "NoDigitizerMode";
			case sdkError.errSDRAMTestFailed:
				return "SDRAMTestFailed";
			case sdkError.errFPGATestRegWriteFailed:
				return "FPGATestRegWriteFailed";
			case sdkError.errFPGATestRegReadFailed:
				return "FPGATestRegReadFailed";
			case sdkError.errXBUSWriteFailed:
				return "XBUSWriteFailed";
			case sdkError.errXBUSReadFailed:
				return "XBUSReadFailed";
			case sdkError.errXBUSTransferNotAllowed:
				return "XBUSTransferNotAllowed";
			case sdkError.errBITHistCaptureFailed:
				return "BITHistCaptureFailed";
			case sdkError.errBITNUCSDRAMFailed:
				return "BITNUCSDRAMFailed";
			case sdkError.errNUCSDRAMBusyTimeout:
				return "NUCSDRAMBusyTimeout";
			case sdkError.errBITNUCFlashFailed:
				return "BITNUCFlashFailed";
			case sdkError.errBITFrameGrabRAMFailed:
				return "BITFrameGrabRAMFailed";
			case sdkError.errBITFPGAControlRegFailed:
				return "BITFPGAControlRegFailed";
			case sdkError.errBITFileSystemFailed:
				return "BITFileSystemFailed";
			case sdkError.errBITVideoFPGAControlRegFailed:
				return "BITVideoFPGAControlRegFailed";
			case sdkError.errBITAGCLUT1Failed:
				return "BITAGCLUT1Failed";
			case sdkError.errBITAGCLUT2Failed:
				return "BITAGCLUT2Failed";
			case sdkError.errBITVideoFrameBufferFailed:
				return "BITVideoFrameBufferFailed";
			case sdkError.errWriteToFileFailed:
				return "WriteToFileFailed";
			case sdkError.errReadFromFileFailed:
				return "ReadFromFileFailed";
			case sdkError.errFileIsNotOpen:
				return "FileIsNotOpen";
			case sdkError.errFileHandleInvalid:
				return "FileHandleInvalid";
			case sdkError.errFileInvalidOperation:
				return "FileInvalidOperation";
			case sdkError.errFileInvalidBlockInChain:
				return "FileInvalidBlockInChain";
			case sdkError.errFrameGrabBusyTimeout:
				return "FrameGrabBusyTimeout";
			case sdkError.errFPGAConfigDoneTimeout:
				return "FPGAConfigDoneTimeout";
			case sdkError.errFileIsOpen:
				return "FileIsOpen";
			case sdkError.errFileIsProtected:
				return "FileIsProtected";
			case sdkError.errMessageQTransmitFail:
				return "MessageQTransmitFail";
			case sdkError.errFileNameInvalid:
				return "FileNameInvalid";
			case sdkError.errEndOfFileReached:
				return "EndOfFileReached";
			case sdkError.errSemaphoreNotCreated:
				return "SemaphoreNotCreated";

			case sdkError.errCacheEntryNotAvailable:
				return "CacheEntryNotAvailable";
			case sdkError.errFlashNotWritten:
				return "FlashNotWritten";
			case sdkError.errBuiltInTestNotFound:
				return "BuiltInTestNotFound";

			case sdkError.errFactoryCalSensorFail:
				return "FactoryCalSensorFail";
			case sdkError.errFactoryCalEdgeNotFound:
				return "FactoryCalEdgeNotFound";
			case sdkError.errFindHomeSensorFail:
				return "FindHomeSensorFail";
			case sdkError.errFindHomeEdgeNotFound:
				return "FindHomeEdgeNotFound";
			case sdkError.errPositionMoveBeforeHomeFound:
				return "PositionMoveBeforeHomeFound";

			case sdkError.errThermistorRatioOutOfRange:
				return "ThermistorRatioOutOfRange";
			case sdkError.errTECOnVolgateTooHigh:
				return "TECOnVolgateTooHigh";
			case sdkError.errTECOffVolgateTooHigh:
				return "TECOffVolgateTooHigh";

			case sdkError.errFPANotInDatabase:
				return "FPANotInDatabase";
			case sdkError.errMaskIndexTooBig:
				return "MaskIndexTooBig";
			case sdkError.errMaskUnsupported:
				return "MaskUnsupported";
			case sdkError.errBadPixelReplaceFailed:
				return "BadPixelReplaceFailed";
			case sdkError.errTooManyBadPixels:
				return "TooManyBadPixels";

			case sdkError.errTableEnterPointNotFound:
				return "TableEnterPointNotFound";
			case sdkError.errForwardTransformAttempt:
				return "ForwardTransformAttempt";
			case sdkError.errCaptureRetryTimeout:
				return "CaptureRetryTimeout";
			case sdkError.errTISAProtocolChecksum:
				return "TISAProtocolChecksum";

			case sdkError.errCCDriverAlreadyOpen:
				return "CCDriverAlreadyOpen";
			case sdkError.errCCPastEndOfCounter:
				return "CCPastEndOfCounter";
			case sdkError.errProtectedFlashArea:
				return "ProtectedFlashArea";

			case sdkError.errINIFileRead:
				return "INIFileRead";

			case sdkError.errDataSizeBad:
				return "DataSizeBad";
			case sdkError.errInvalidModuleID:
				return "InvalidModuleID";
			case sdkError.errUnsupportedCommandID:
				return "UnsupportedCommandID";
			case sdkError.errUnsupportedGetID:
				return "UnsupportedGetID";
			case sdkError.errUnsupportedSetID:
				return "UnsupportedSetID";
			case sdkError.errInvalidAccessCode:
				return "InvalidAccessCode";
			case sdkError.errInvalidVideoTapPoint:
				return "InvalidVideoTapPoint";
			case sdkError.errNUCCoeffChecksumFail:
				return "errNUCCoeffChecksumFail";
			case sdkError.errLineEndNotFound:
				return "errLineEndNotFound";
			case sdkError.errWriteToHwFailed:
				return "errWriteToHwFailed";
			case sdkError.errReadFromHwFailed:
				return "errReadFromHwFailed";
			case sdkError.errI2CStartFailure:
				return "errI2CStartFailure";
			case sdkError.errI2CRepeatStartFailure:
				return "errI2CRepeatStartFailure";
			case sdkError.errI2CReadFailure:
				return "errI2CReadFailure";
			case sdkError.errI2CWriteFailure:
				return "errI2CWriteFailure";
			case sdkError.errI2CReadAddrFailure:
				return "errI2CReadAddrFailure";
			case sdkError.errI2CWriteAddrFailure:
				return "errI2CWriteAddrFailure";
			case sdkError.errInvalidFlagCommand:
				return "errInvalidFlagCommand";
			case sdkError.errInvalidFlagState:
				return "errInvalidFlagState";
			case sdkError.errInvalidFlagStepoint:
				return "errInvalidFlagStepoint";
			case sdkError.errInvalidNUCFileType:
				return "errInvalidNUCFileType";
			case sdkError.errIntegrationTimeTooLong:
				return "errIntegrationTimeTooLong";
			case sdkError.errFramePeriodTooShort:
				return "errFramePeriodTooShort";
			case sdkError.errConfigSaveFailed:
				return "errConfigSaveFailed";
			case sdkError.errConfigRestoreFailed:
				return "errConfigRestoreFailed";
			case sdkError.errInvalidAGCFileType:
				return "errInvalidAGCFileType";
			case sdkError.errReadOnlyParameter:
				return "errReadOnlyParameter";
			case sdkError.errWriteOnlyParameter:
				return "errWriteOnlyParameter";
			case sdkError.errFileBlockSizeZero:
				return "errFileBlockSizeZero";
			case sdkError.errTransferBlockSizeTooLarge:
				return "errTransferBlockSizeTooLarge";
			case sdkError.errFileNameNull:
				return "errFileNameNull";
			case sdkError.errFileSizeZero:
				return "errFileSizeZero";
			case sdkError.errFileInvalidTransferMode:
				return "errFileInvalidTransferMode";
			case sdkError.errDirListNotComplete:
				return "errDirListNotComplete";
			case sdkError.errDirNameTooLong:
				return "errDirNameTooLong";
			case sdkError.errDirNameTooShort:
				return "errDirNameTooShort";
			case sdkError.errDirInvalidBlockCount:
				return "errDirInvalidBlockCount";
			case sdkError.errDirInvalidDriveName:
				return "errDirInvalidDriveName";
			case sdkError.errCommandSetFailure:
				return "errCommandSetFailure";
			case sdkError.errInvalidHistogramTap:
				return "errInvalidHistogramTap";
			default:
				return "Unknown failure (0x" + Error.ToString("X4") + ")";
			}
		}

		/// <summary>
		/// Examines an error code and throws an appropriate exception if need be.
		/// </summary>
		/// <param name="Error">SDK error code.</param>
		public static void checkError(int Error)
		{
            if ((sdkError)Error != sdkError.errOk)
            {
                System.Diagnostics.Debug.WriteLine(errorString(Error));
                if ( (Error == 0xFFFF) || (Error == 0x0112) || ( (Error > 0x0101) && (Error < 0x010f) ) )
                    throw new SDKException(Error);
            }
        }
	}
}
