// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PushserviceToken.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from PushserviceToken.proto</summary>
public static partial class PushServiceTokenReflection {

  #region Descriptor
  /// <summary>File descriptor for PushserviceToken.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PushServiceTokenReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChZQdXNoc2VydmljZVRva2VuLnByb3RvIqwCChBQdXNoU2VydmljZVRva2Vu",
          "EiwKCHB1c2hUeXBlGAEgASgOMhouUHVzaFNlcnZpY2VUb2tlbi5QdXNoVHlw",
          "ZRINCgV0b2tlbhgCIAEoDBIVCg1pc1Bhc3NUaHJvdWdoGAMgASgIIsMBCghQ",
          "dXNoVHlwZRIUChBrUHVzaFR5cGVJbnZhbGlkEAASEQoNa1B1c2hUeXBlQVBO",
          "UxABEhMKD2tQdXNoVHlwZVhtUHVzaBACEhMKD2tQdXNoVHlwZUpnUHVzaBAD",
          "EhMKD2tQdXNoVHlwZUd0UFVzaBAEEhMKD2tQdXNoVHlwZU9wUHVzaBAFEhMK",
          "D2tQdXNoVFlwZVZ2UHVzaBAGEhMKD2tQdXNoVHlwZUh3UHVzaBAHEhAKDGtQ",
          "dXNoVFlwZUZjbRAIYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PushServiceToken), global::PushServiceToken.Parser, new[]{ "PushType", "Token", "IsPassThrough" }, null, new[]{ typeof(global::PushServiceToken.Types.PushType) }, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PushServiceToken : pb::IMessage<PushServiceToken> {
  private static readonly pb::MessageParser<PushServiceToken> _parser = new pb::MessageParser<PushServiceToken>(() => new PushServiceToken());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<PushServiceToken> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PushServiceTokenReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PushServiceToken() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PushServiceToken(PushServiceToken other) : this() {
    pushType_ = other.pushType_;
    token_ = other.token_;
    isPassThrough_ = other.isPassThrough_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PushServiceToken Clone() {
    return new PushServiceToken(this);
  }

  /// <summary>Field number for the "pushType" field.</summary>
  public const int PushTypeFieldNumber = 1;
  private global::PushServiceToken.Types.PushType pushType_ = global::PushServiceToken.Types.PushType.KPushTypeInvalid;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::PushServiceToken.Types.PushType PushType {
    get { return pushType_; }
    set {
      pushType_ = value;
    }
  }

  /// <summary>Field number for the "token" field.</summary>
  public const int TokenFieldNumber = 2;
  private pb::ByteString token_ = pb::ByteString.Empty;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pb::ByteString Token {
    get { return token_; }
    set {
      token_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "isPassThrough" field.</summary>
  public const int IsPassThroughFieldNumber = 3;
  private bool isPassThrough_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool IsPassThrough {
    get { return isPassThrough_; }
    set {
      isPassThrough_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as PushServiceToken);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(PushServiceToken other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (PushType != other.PushType) return false;
    if (Token != other.Token) return false;
    if (IsPassThrough != other.IsPassThrough) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (PushType != global::PushServiceToken.Types.PushType.KPushTypeInvalid) hash ^= PushType.GetHashCode();
    if (Token.Length != 0) hash ^= Token.GetHashCode();
    if (IsPassThrough != false) hash ^= IsPassThrough.GetHashCode();
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
    if (PushType != global::PushServiceToken.Types.PushType.KPushTypeInvalid) {
      output.WriteRawTag(8);
      output.WriteEnum((int) PushType);
    }
    if (Token.Length != 0) {
      output.WriteRawTag(18);
      output.WriteBytes(Token);
    }
    if (IsPassThrough != false) {
      output.WriteRawTag(24);
      output.WriteBool(IsPassThrough);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (PushType != global::PushServiceToken.Types.PushType.KPushTypeInvalid) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) PushType);
    }
    if (Token.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeBytesSize(Token);
    }
    if (IsPassThrough != false) {
      size += 1 + 1;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(PushServiceToken other) {
    if (other == null) {
      return;
    }
    if (other.PushType != global::PushServiceToken.Types.PushType.KPushTypeInvalid) {
      PushType = other.PushType;
    }
    if (other.Token.Length != 0) {
      Token = other.Token;
    }
    if (other.IsPassThrough != false) {
      IsPassThrough = other.IsPassThrough;
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
        case 8: {
          PushType = (global::PushServiceToken.Types.PushType) input.ReadEnum();
          break;
        }
        case 18: {
          Token = input.ReadBytes();
          break;
        }
        case 24: {
          IsPassThrough = input.ReadBool();
          break;
        }
      }
    }
  }

  #region Nested types
  /// <summary>Container for nested types declared in the PushServiceToken message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static partial class Types {
    public enum PushType {
      [pbr::OriginalName("kPushTypeInvalid")] KPushTypeInvalid = 0,
      [pbr::OriginalName("kPushTypeAPNS")] KPushTypeApns = 1,
      [pbr::OriginalName("kPushTypeXmPush")] KPushTypeXmPush = 2,
      [pbr::OriginalName("kPushTypeJgPush")] KPushTypeJgPush = 3,
      [pbr::OriginalName("kPushTypeGtPUsh")] KPushTypeGtPush = 4,
      [pbr::OriginalName("kPushTypeOpPush")] KPushTypeOpPush = 5,
      [pbr::OriginalName("kPushTYpeVvPush")] KPushTypeVvPush = 6,
      [pbr::OriginalName("kPushTypeHwPush")] KPushTypeHwPush = 7,
      [pbr::OriginalName("kPushTYpeFcm")] KPushTypeFcm = 8,
    }

  }
  #endregion

}

#endregion


#endregion Designer generated code