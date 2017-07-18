import tps_message_pb2
import fight_room_manager
import msgpack

def action_data(conn, msg):
    rst = tps_message_pb2.CS_Fight_Message()
    rst.ParseFromString(msg)
    fight_room_manager.transport_fight_msg(conn, rst.action_data)


def rpc_command(conn, msg):
    rst = tps_message_pb2.CS_RPC_RunRequest()
    rst.ParseFromString(msg)
    data = msgpack.unpackb(rst.parameters)
    func_name = data["func_name"]
    if func_name == None:
        print "rpc_command error:no func_name"
        print data
        assert False
    func = getattr(fight_room_manager, func_name)

    func(conn, data)