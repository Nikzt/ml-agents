# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: communicator/agent_action.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='communicator/agent_action.proto',
  package='communicator',
  syntax='proto3',
  serialized_pb=_b('\n\x1f\x63ommunicator/agent_action.proto\x12\x0c\x63ommunicator\"M\n\x0b\x41gentAction\x12\x16\n\x0evector_actions\x18\x01 \x03(\x02\x12\x14\n\x0ctext_actions\x18\x02 \x01(\t\x12\x10\n\x08memories\x18\x03 \x03(\x02\x42\x18\xaa\x02\x15MLAgents.Communicatorb\x06proto3')
)




_AGENTACTION = _descriptor.Descriptor(
  name='AgentAction',
  full_name='communicator.AgentAction',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='vector_actions', full_name='communicator.AgentAction.vector_actions', index=0,
      number=1, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='text_actions', full_name='communicator.AgentAction.text_actions', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='memories', full_name='communicator.AgentAction.memories', index=2,
      number=3, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=49,
  serialized_end=126,
)

DESCRIPTOR.message_types_by_name['AgentAction'] = _AGENTACTION
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

AgentAction = _reflection.GeneratedProtocolMessageType('AgentAction', (_message.Message,), dict(
  DESCRIPTOR = _AGENTACTION,
  __module__ = 'communicator.agent_action_pb2'
  # @@protoc_insertion_point(class_scope:communicator.AgentAction)
  ))
_sym_db.RegisterMessage(AgentAction)


DESCRIPTOR.has_options = True
DESCRIPTOR._options = _descriptor._ParseOptions(descriptor_pb2.FileOptions(), _b('\252\002\025MLAgents.Communicator'))
# @@protoc_insertion_point(module_scope)