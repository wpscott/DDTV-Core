// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PacketHeader.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from PacketHeader.proto</summary>
public static partial class PacketHeaderReflection {

  #region Descriptor
  /// <summary>File descriptor for PacketHeader.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PacketHeaderReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChJQYWNrZXRIZWFkZXIucHJvdG8aD1Rva2VuSW5mby5wcm90byK0BAoMUGFj",
          "a2V0SGVhZGVyEg0KBWFwcElkGAEgASgFEgsKA3VpZBgCIAEoAxISCgppbnN0",
          "YW5jZUlkGAMgASgDEg0KBWZsYWdzGAUgASgNEjAKDGVuY29kaW5nVHlwZRgG",
          "IAEoDjIaLlBhY2tldEhlYWRlci5FbmNvZGluZ1R5cGUSGQoRZGVjb2RlZFBh",
          "eWxvYWRMZW4YByABKA0SNAoOZW5jcnlwdGlvbk1vZGUYCCABKA4yHC5QYWNr",
          "ZXRIZWFkZXIuRW5jcnlwdGlvbk1vZGUSHQoJdG9rZW5JbmZvGAkgASgLMgou",
          "VG9rZW5JbmZvEg0KBXNlcUlkGAogASgDEicKCGZlYXR1cmVzGAsgAygOMhUu",
          "UGFja2V0SGVhZGVyLkZlYXR1cmUSCwoDa3BuGAwgASgJIj8KBUZsYWdzEhAK",
          "DGtEaXJVcHN0cmVhbRAAEhIKDmtEaXJEb3duc3RyZWFtEAESDAoIa0Rpck1h",
          "c2sQARoCEAEiMwoMRW5jb2RpbmdUeXBlEhEKDWtFbmNvZGluZ05vbmUQABIQ",
          "CgxrRW5jb2RpbmdMejQQASJdCg5FbmNyeXB0aW9uTW9kZRITCg9rRW5jcnlw",
          "dGlvbk5vbmUQABIbChdrRW5jcnlwdGlvblNlcnZpY2VUb2tlbhABEhkKFWtF",
          "bmNyeXB0aW9uU2Vzc2lvbktleRACIikKB0ZlYXR1cmUSDAoIa1Jlc2VydmUQ",
          "ABIQCgxrQ29tcHJlc3NMejQQAWIGcHJvdG8z"));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { global::TokenInfoReflection.Descriptor, },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PacketHeader), global::PacketHeader.Parser, new[]{ "AppId", "Uid", "InstanceId", "Flags", "EncodingType", "DecodedPayloadLen", "EncryptionMode", "TokenInfo", "SeqId", "Features", "Kpn" }, null, new[]{ typeof(global::PacketHeader.Types.Flags), typeof(global::PacketHeader.Types.EncodingType), typeof(global::PacketHeader.Types.EncryptionMode), typeof(global::PacketHeader.Types.Feature) }, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PacketHeader : pb::IMessage<PacketHeader> {
  private static readonly pb::MessageParser<PacketHeader> _parser = new pb::MessageParser<PacketHeader>(() => new PacketHeader());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<PacketHeader> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PacketHeaderReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PacketHeader() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PacketHeader(PacketHeader other) : this() {
    appId_ = other.appId_;
    uid_ = other.uid_;
    instanceId_ = other.instanceId_;
    flags_ = other.flags_;
    encodingType_ = other.encodingType_;
    decodedPayloadLen_ = other.decodedPayloadLen_;
    encryptionMode_ = other.encryptionMode_;
    tokenInfo_ = other.tokenInfo_ != null ? other.tokenInfo_.Clone() : null;
    seqId_ = other.seqId_;
    features_ = other.features_.Clone();
    kpn_ = other.kpn_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PacketHeader Clone() {
    return new PacketHeader(this);
  }

  /// <summary>Field number for the "appId" field.</summary>
  public const int AppIdFieldNumber = 1;
  private int appId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int AppId {
    get { return appId_; }
    set {
      appId_ = value;
    }
  }

  /// <summary>Field number for the "uid" field.</summary>
  public const int UidFieldNumber = 2;
  private long uid_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public long Uid {
    get { return uid_; }
    set {
      uid_ = value;
    }
  }

  /// <summary>Field number for the "instanceId" field.</summary>
  public const int InstanceIdFieldNumber = 3;
  private long instanceId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public long InstanceId {
    get { return instanceId_; }
    set {
      instanceId_ = value;
    }
  }

  /// <summary>Field number for the "flags" field.</summary>
  public const int FlagsFieldNumber = 5;
  private uint flags_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public uint Flags {
    get { return flags_; }
    set {
      flags_ = value;
    }
  }

  /// <summary>Field number for the "encodingType" field.</summary>
  public const int EncodingTypeFieldNumber = 6;
  private global::PacketHeader.Types.EncodingType encodingType_ = global::PacketHeader.Types.EncodingType.KEncodingNone;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::PacketHeader.Types.EncodingType EncodingType {
    get { return encodingType_; }
    set {
      encodingType_ = value;
    }
  }

  /// <summary>Field number for the "decodedPayloadLen" field.</summary>
  public const int DecodedPayloadLenFieldNumber = 7;
  private uint decodedPayloadLen_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public uint DecodedPayloadLen {
    get { return decodedPayloadLen_; }
    set {
      decodedPayloadLen_ = value;
    }
  }

  /// <summary>Field number for the "encryptionMode" field.</summary>
  public const int EncryptionModeFieldNumber = 8;
  private global::PacketHeader.Types.EncryptionMode encryptionMode_ = global::PacketHeader.Types.EncryptionMode.KEncryptionNone;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::PacketHeader.Types.EncryptionMode EncryptionMode {
    get { return encryptionMode_; }
    set {
      encryptionMode_ = value;
    }
  }

  /// <summary>Field number for the "tokenInfo" field.</summary>
  public const int TokenInfoFieldNumber = 9;
  private global::TokenInfo tokenInfo_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::TokenInfo TokenInfo {
    get { return tokenInfo_; }
    set {
      tokenInfo_ = value;
    }
  }

  /// <summary>Field number for the "seqId" field.</summary>
  public const int SeqIdFieldNumber = 10;
  private long seqId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public long SeqId {
    get { return seqId_; }
    set {
      seqId_ = value;
    }
  }

  /// <summary>Field number for the "features" field.</summary>
  public const int FeaturesFieldNumber = 11;
  private static readonly pb::FieldCodec<global::PacketHeader.Types.Feature> _repeated_features_codec
      = pb::FieldCodec.ForEnum(90, x => (int) x, x => (global::PacketHeader.Types.Feature) x);
  private readonly pbc::RepeatedField<global::PacketHeader.Types.Feature> features_ = new pbc::RepeatedField<global::PacketHeader.Types.Feature>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::PacketHeader.Types.Feature> Features {
    get { return features_; }
  }

  /// <summary>Field number for the "kpn" field.</summary>
  public const int KpnFieldNumber = 12;
  private string kpn_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Kpn {
    get { return kpn_; }
    set {
      kpn_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as PacketHeader);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(PacketHeader other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (AppId != other.AppId) return false;
    if (Uid != other.Uid) return false;
    if (InstanceId != other.InstanceId) return false;
    if (Flags != other.Flags) return false;
    if (EncodingType != other.EncodingType) return false;
    if (DecodedPayloadLen != other.DecodedPayloadLen) return false;
    if (EncryptionMode != other.EncryptionMode) return false;
    if (!object.Equals(TokenInfo, other.TokenInfo)) return false;
    if (SeqId != other.SeqId) return false;
    if(!features_.Equals(other.features_)) return false;
    if (Kpn != other.Kpn) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (AppId != 0) hash ^= AppId.GetHashCode();
    if (Uid != 0L) hash ^= Uid.GetHashCode();
    if (InstanceId != 0L) hash ^= InstanceId.GetHashCode();
    if (Flags != 0) hash ^= Flags.GetHashCode();
    if (EncodingType != global::PacketHeader.Types.EncodingType.KEncodingNone) hash ^= EncodingType.GetHashCode();
    if (DecodedPayloadLen != 0) hash ^= DecodedPayloadLen.GetHashCode();
    if (EncryptionMode != global::PacketHeader.Types.EncryptionMode.KEncryptionNone) hash ^= EncryptionMode.GetHashCode();
    if (tokenInfo_ != null) hash ^= TokenInfo.GetHashCode();
    if (SeqId != 0L) hash ^= SeqId.GetHashCode();
    hash ^= features_.GetHashCode();
    if (Kpn.Length != 0) hash ^= Kpn.GetHashCode();
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
    if (AppId != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(AppId);
    }
    if (Uid != 0L) {
      output.WriteRawTag(16);
      output.WriteInt64(Uid);
    }
    if (InstanceId != 0L) {
      output.WriteRawTag(24);
      output.WriteInt64(InstanceId);
    }
    if (Flags != 0) {
      output.WriteRawTag(40);
      output.WriteUInt32(Flags);
    }
    if (EncodingType != global::PacketHeader.Types.EncodingType.KEncodingNone) {
      output.WriteRawTag(48);
      output.WriteEnum((int) EncodingType);
    }
    if (DecodedPayloadLen != 0) {
      output.WriteRawTag(56);
      output.WriteUInt32(DecodedPayloadLen);
    }
    if (EncryptionMode != global::PacketHeader.Types.EncryptionMode.KEncryptionNone) {
      output.WriteRawTag(64);
      output.WriteEnum((int) EncryptionMode);
    }
    if (tokenInfo_ != null) {
      output.WriteRawTag(74);
      output.WriteMessage(TokenInfo);
    }
    if (SeqId != 0L) {
      output.WriteRawTag(80);
      output.WriteInt64(SeqId);
    }
    features_.WriteTo(output, _repeated_features_codec);
    if (Kpn.Length != 0) {
      output.WriteRawTag(98);
      output.WriteString(Kpn);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (AppId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(AppId);
    }
    if (Uid != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(Uid);
    }
    if (InstanceId != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(InstanceId);
    }
    if (Flags != 0) {
      size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Flags);
    }
    if (EncodingType != global::PacketHeader.Types.EncodingType.KEncodingNone) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) EncodingType);
    }
    if (DecodedPayloadLen != 0) {
      size += 1 + pb::CodedOutputStream.ComputeUInt32Size(DecodedPayloadLen);
    }
    if (EncryptionMode != global::PacketHeader.Types.EncryptionMode.KEncryptionNone) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) EncryptionMode);
    }
    if (tokenInfo_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(TokenInfo);
    }
    if (SeqId != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(SeqId);
    }
    size += features_.CalculateSize(_repeated_features_codec);
    if (Kpn.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Kpn);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(PacketHeader other) {
    if (other == null) {
      return;
    }
    if (other.AppId != 0) {
      AppId = other.AppId;
    }
    if (other.Uid != 0L) {
      Uid = other.Uid;
    }
    if (other.InstanceId != 0L) {
      InstanceId = other.InstanceId;
    }
    if (other.Flags != 0) {
      Flags = other.Flags;
    }
    if (other.EncodingType != global::PacketHeader.Types.EncodingType.KEncodingNone) {
      EncodingType = other.EncodingType;
    }
    if (other.DecodedPayloadLen != 0) {
      DecodedPayloadLen = other.DecodedPayloadLen;
    }
    if (other.EncryptionMode != global::PacketHeader.Types.EncryptionMode.KEncryptionNone) {
      EncryptionMode = other.EncryptionMode;
    }
    if (other.tokenInfo_ != null) {
      if (tokenInfo_ == null) {
        TokenInfo = new global::TokenInfo();
      }
      TokenInfo.MergeFrom(other.TokenInfo);
    }
    if (other.SeqId != 0L) {
      SeqId = other.SeqId;
    }
    features_.Add(other.features_);
    if (other.Kpn.Length != 0) {
      Kpn = other.Kpn;
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
          AppId = input.ReadInt32();
          break;
        }
        case 16: {
          Uid = input.ReadInt64();
          break;
        }
        case 24: {
          InstanceId = input.ReadInt64();
          break;
        }
        case 40: {
          Flags = input.ReadUInt32();
          break;
        }
        case 48: {
          EncodingType = (global::PacketHeader.Types.EncodingType) input.ReadEnum();
          break;
        }
        case 56: {
          DecodedPayloadLen = input.ReadUInt32();
          break;
        }
        case 64: {
          EncryptionMode = (global::PacketHeader.Types.EncryptionMode) input.ReadEnum();
          break;
        }
        case 74: {
          if (tokenInfo_ == null) {
            TokenInfo = new global::TokenInfo();
          }
          input.ReadMessage(TokenInfo);
          break;
        }
        case 80: {
          SeqId = input.ReadInt64();
          break;
        }
        case 90:
        case 88: {
          features_.AddEntriesFrom(input, _repeated_features_codec);
          break;
        }
        case 98: {
          Kpn = input.ReadString();
          break;
        }
      }
    }
  }

  #region Nested types
  /// <summary>Container for nested types declared in the PacketHeader message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static partial class Types {
    public enum Flags {
      [pbr::OriginalName("kDirUpstream")] KDirUpstream = 0,
      [pbr::OriginalName("kDirDownstream")] KDirDownstream = 1,
      [pbr::OriginalName("kDirMask", PreferredAlias = false)] KDirMask = 1,
    }

    public enum EncodingType {
      [pbr::OriginalName("kEncodingNone")] KEncodingNone = 0,
      [pbr::OriginalName("kEncodingLz4")] KEncodingLz4 = 1,
    }

    public enum EncryptionMode {
      [pbr::OriginalName("kEncryptionNone")] KEncryptionNone = 0,
      [pbr::OriginalName("kEncryptionServiceToken")] KEncryptionServiceToken = 1,
      [pbr::OriginalName("kEncryptionSessionKey")] KEncryptionSessionKey = 2,
    }

    public enum Feature {
      [pbr::OriginalName("kReserve")] KReserve = 0,
      [pbr::OriginalName("kCompressLz4")] KCompressLz4 = 1,
    }

  }
  #endregion

}

#endregion


#endregion Designer generated code
