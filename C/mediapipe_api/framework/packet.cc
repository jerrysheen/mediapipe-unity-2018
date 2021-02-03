#include <string>
#include <utility>
#include "mediapipe_api/framework/packet.h"

MpReturnCode mp_Packet__(mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet();
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

void mp_Packet__delete(mediapipe::Packet* packet) {
  delete packet;
}

MpReturnCode mp_Packet__At__Rtimestamp(mediapipe::Packet* packet, mediapipe::Timestamp* timestamp, mediapipe::Packet** packet_out) {
  TRY {
    // not move but copy
    *packet_out = new mediapipe::Packet { packet->At(*timestamp) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__ValidateAsProtoMessageLite(mediapipe::Packet* packet, mediapipe::Status** status_out) {
  TRY {
    *status_out = new mediapipe::Status { packet->ValidateAsProtoMessageLite() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__Timestamp(mediapipe::Packet* packet, mediapipe::Timestamp** timestamp_out) {
  TRY {
    *timestamp_out = new mediapipe::Timestamp { packet->Timestamp() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__DebugString(mediapipe::Packet* packet, const char** str_out) {
  TRY {
    *str_out = strcpy_to_heap(packet->DebugString());
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__RegisteredTypeName(mediapipe::Packet* packet, const char** str_out) {
  TRY {
    *str_out = strcpy_to_heap(packet->RegisteredTypeName());
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__DebugTypeName(mediapipe::Packet* packet, const char** str_out) {
  TRY {
    *str_out = strcpy_to_heap(packet->DebugTypeName());
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

// BoolPacket
MpReturnCode mp__MakeBoolPacket__b(bool value, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<bool>(value) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp__MakeBoolPacket_At__b_Rtimestamp(bool value, mediapipe::Timestamp* timestamp, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<bool>(value).At(*timestamp) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__GetBool(mediapipe::Packet* packet, bool* value_out) {
  TRY_ALL {
    *value_out = packet->Get<bool>();
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_ALL
}

MpReturnCode mp_Packet__ValidateAsBool(mediapipe::Packet* packet, mediapipe::Status** status_out) {
  TRY {
    *status_out = new mediapipe::Status { packet->ValidateAsType<bool>() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

// FloatPacket
MpReturnCode mp__MakeFloatPacket__f(float value, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<float>(value) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp__MakeFloatPacket_At__f_Rtimestamp(float value, mediapipe::Timestamp* timestamp, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<float>(value).At(*timestamp) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__GetFloat(mediapipe::Packet* packet, float* value_out) {
  TRY_ALL {
    *value_out = packet->Get<float>();
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_ALL
}

MpReturnCode mp_Packet__ValidateAsFloat(mediapipe::Packet* packet, mediapipe::Status** status_out) {
  TRY {
    *status_out = new mediapipe::Status { packet->ValidateAsType<float>() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

// IntPacket
MpReturnCode mp__MakeIntPacket__i(int value, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<int>(value) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp__MakeIntPacket_At__i_Rtimestamp(int value, mediapipe::Timestamp* timestamp, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<int>(value).At(*timestamp) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__GetInt(mediapipe::Packet* packet, int* value_out) {
  TRY_ALL {
    *value_out = packet->Get<int>();
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_ALL
}

MpReturnCode mp_Packet__ValidateAsInt(mediapipe::Packet* packet, mediapipe::Status** status_out) {
  TRY {
    *status_out = new mediapipe::Status { packet->ValidateAsType<int>() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

// StringPacket
MpReturnCode mp__MakeStringPacket__PKc(const char* str, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<std::string>(std::string(str)) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp__MakeStringPacket_At__PKc_Rtimestamp(const char* str, mediapipe::Timestamp* timestamp, mediapipe::Packet** packet_out) {
  TRY {
    *packet_out = new mediapipe::Packet { mediapipe::MakePacket<std::string>(std::string(str)).At(*timestamp) };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_Packet__GetString(mediapipe::Packet* packet, const char** value_out) {
  TRY_ALL {
    *value_out = strcpy_to_heap(packet->Get<std::string>());
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_ALL
}

MpReturnCode mp_Packet__ValidateAsString(mediapipe::Packet* packet, mediapipe::Status** status_out) {
  TRY {
    *status_out = new mediapipe::Status { packet->ValidateAsType<std::string>() };
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

/** SidePacket */
MpReturnCode mp_SidePacket__(SidePacket** side_packet_out) {
  TRY {
    *side_packet_out = new SidePacket();
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

void mp_SidePacket__delete(SidePacket* side_packet) {
  delete side_packet;
}

MpReturnCode mp_SidePacket__emplace__PKc_Rpacket(SidePacket* side_packet, const char* key, mediapipe::Packet* packet) {
  TRY {
    side_packet->emplace(std::string(key), std::move(*packet));
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

MpReturnCode mp_SidePacket__at__PKc(SidePacket* side_packet, const char* key, mediapipe::Packet** packet_out) {
  TRY {
    try {
      auto packet = side_packet->at(std::string(key));
      // copy
      *packet_out = new mediapipe::Packet { packet };
      RETURN_CODE(MpReturnCode::Success);
    } catch (std::out_of_range&) {
      *packet_out = nullptr;
      RETURN_CODE(MpReturnCode::Success);
    }
  } CATCH_EXCEPTION
}

MpReturnCode mp_SidePacket__erase__PKc(SidePacket* side_packet, const char* key, int* count_out) {
  TRY {
    *count_out = side_packet->erase(std::string(key));
    RETURN_CODE(MpReturnCode::Success);
  } CATCH_EXCEPTION
}

void mp_SidePacket__clear(SidePacket* side_packet) {
  side_packet->clear();
}

int mp_SidePacket__size(SidePacket* side_packet) {
  return side_packet->size();
}