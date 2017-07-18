from struct import pack, unpack
import tps_message_pb2
import const
import msgpack

def send(conn, opcode, message):
    total_len = len(message) + 6
    conn.send_data(''.join([pack('<I', total_len), pack('<HHH', 0, 0, opcode), message]))
    print "send", message

def create_rpc_data(message):
    msg = tps_message_pb2.SC_RPC_RunRequest()
    msg.opcode = const.RPC_CODE["SC_MESSAGE_RPC"]
    msg.parameters = msgpack.packb(message)
    return msg.SerializeToString()