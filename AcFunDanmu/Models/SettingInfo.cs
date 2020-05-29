// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: SettingInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from SettingInfo.proto</summary>
public static partial class SettingInfoReflection {

  #region Descriptor
  /// <summary>File descriptor for SettingInfo.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static SettingInfoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChFTZXR0aW5nSW5mby5wcm90byIvCgtTZXR0aW5nSW5mbxIOCgZsb2NhbGUY",
          "ASABKAkSEAoIdGltZXpvbmUYAiABKBFiBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::SettingInfo), global::SettingInfo.Parser, new[]{ "Locale", "Timezone" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class SettingInfo : pb::IMessage<SettingInfo> {
  private static readonly pb::MessageParser<SettingInfo> _parser = new pb::MessageParser<SettingInfo>(() => new SettingInfo());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<SettingInfo> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::SettingInfoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SettingInfo() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SettingInfo(SettingInfo other) : this() {
    locale_ = other.locale_;
    timezone_ = other.timezone_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public SettingInfo Clone() {
    return new SettingInfo(this);
  }

  /// <summary>Field number for the "locale" field.</summary>
  public const int LocaleFieldNumber = 1;
  private string locale_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Locale {
    get { return locale_; }
    set {
      locale_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "timezone" field.</summary>
  public const int TimezoneFieldNumber = 2;
  private int timezone_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Timezone {
    get { return timezone_; }
    set {
      timezone_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as SettingInfo);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(SettingInfo other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Locale != other.Locale) return false;
    if (Timezone != other.Timezone) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Locale.Length != 0) hash ^= Locale.GetHashCode();
    if (Timezone != 0) hash ^= Timezone.GetHashCode();
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
    if (Locale.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Locale);
    }
    if (Timezone != 0) {
      output.WriteRawTag(16);
      output.WriteSInt32(Timezone);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Locale.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Locale);
    }
    if (Timezone != 0) {
      size += 1 + pb::CodedOutputStream.ComputeSInt32Size(Timezone);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(SettingInfo other) {
    if (other == null) {
      return;
    }
    if (other.Locale.Length != 0) {
      Locale = other.Locale;
    }
    if (other.Timezone != 0) {
      Timezone = other.Timezone;
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
          Locale = input.ReadString();
          break;
        }
        case 16: {
          Timezone = input.ReadSInt32();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
