// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: AppInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from AppInfo.proto</summary>
public static partial class AppInfoReflection {

  #region Descriptor
  /// <summary>File descriptor for AppInfo.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static AppInfoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg1BcHBJbmZvLnByb3RvIm0KB0FwcEluZm8SDwoHYXBwTmFtZRgBIAEoCRIS",
          "CgphcHBWZXJzaW9uGAIgASgJEhIKCmFwcENoYW5uZWwYAyABKAkSEgoKc2Rr",
          "VmVyc2lvbhgEIAEoCRIVCg1leHRlbnNpb25JbmZvGAsgASgJYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::AppInfo), global::AppInfo.Parser, new[]{ "AppName", "AppVersion", "AppChannel", "SdkVersion", "ExtensionInfo" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class AppInfo : pb::IMessage<AppInfo> {
  private static readonly pb::MessageParser<AppInfo> _parser = new pb::MessageParser<AppInfo>(() => new AppInfo());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<AppInfo> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::AppInfoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AppInfo() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AppInfo(AppInfo other) : this() {
    appName_ = other.appName_;
    appVersion_ = other.appVersion_;
    appChannel_ = other.appChannel_;
    sdkVersion_ = other.sdkVersion_;
    extensionInfo_ = other.extensionInfo_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AppInfo Clone() {
    return new AppInfo(this);
  }

  /// <summary>Field number for the "appName" field.</summary>
  public const int AppNameFieldNumber = 1;
  private string appName_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string AppName {
    get { return appName_; }
    set {
      appName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "appVersion" field.</summary>
  public const int AppVersionFieldNumber = 2;
  private string appVersion_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string AppVersion {
    get { return appVersion_; }
    set {
      appVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "appChannel" field.</summary>
  public const int AppChannelFieldNumber = 3;
  private string appChannel_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string AppChannel {
    get { return appChannel_; }
    set {
      appChannel_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "sdkVersion" field.</summary>
  public const int SdkVersionFieldNumber = 4;
  private string sdkVersion_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string SdkVersion {
    get { return sdkVersion_; }
    set {
      sdkVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "extensionInfo" field.</summary>
  public const int ExtensionInfoFieldNumber = 11;
  private string extensionInfo_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string ExtensionInfo {
    get { return extensionInfo_; }
    set {
      extensionInfo_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as AppInfo);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(AppInfo other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (AppName != other.AppName) return false;
    if (AppVersion != other.AppVersion) return false;
    if (AppChannel != other.AppChannel) return false;
    if (SdkVersion != other.SdkVersion) return false;
    if (ExtensionInfo != other.ExtensionInfo) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (AppName.Length != 0) hash ^= AppName.GetHashCode();
    if (AppVersion.Length != 0) hash ^= AppVersion.GetHashCode();
    if (AppChannel.Length != 0) hash ^= AppChannel.GetHashCode();
    if (SdkVersion.Length != 0) hash ^= SdkVersion.GetHashCode();
    if (ExtensionInfo.Length != 0) hash ^= ExtensionInfo.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (AppName.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(AppName);
    }
    if (AppVersion.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(AppVersion);
    }
    if (AppChannel.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(AppChannel);
    }
    if (SdkVersion.Length != 0) {
      output.WriteRawTag(34);
      output.WriteString(SdkVersion);
    }
    if (ExtensionInfo.Length != 0) {
      output.WriteRawTag(90);
      output.WriteString(ExtensionInfo);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (AppName.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(AppName);
    }
    if (AppVersion.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(AppVersion);
    }
    if (AppChannel.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(AppChannel);
    }
    if (SdkVersion.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(SdkVersion);
    }
    if (ExtensionInfo.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(ExtensionInfo);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(AppInfo other) {
    if (other == null) {
      return;
    }
    if (other.AppName.Length != 0) {
      AppName = other.AppName;
    }
    if (other.AppVersion.Length != 0) {
      AppVersion = other.AppVersion;
    }
    if (other.AppChannel.Length != 0) {
      AppChannel = other.AppChannel;
    }
    if (other.SdkVersion.Length != 0) {
      SdkVersion = other.SdkVersion;
    }
    if (other.ExtensionInfo.Length != 0) {
      ExtensionInfo = other.ExtensionInfo;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          AppName = input.ReadString();
          break;
        }
        case 18: {
          AppVersion = input.ReadString();
          break;
        }
        case 26: {
          AppChannel = input.ReadString();
          break;
        }
        case 34: {
          SdkVersion = input.ReadString();
          break;
        }
        case 90: {
          ExtensionInfo = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code