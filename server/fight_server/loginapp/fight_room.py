import tps_message_pb2
import const
from network import send, create_rpc_data
import timer


room_index = {}


def get_room_id_by_player_id(player_id):
    return room_index[player_id]


class fight_room(object):
    def __init__(self, room_id, conn_list):
        self.frame_data = tps_message_pb2.SC_Fight_Message()
        self.frame_data.frame = 1
        self.order_list = []
        self.conn_list = conn_list
        ready_list = {}
        self.ready_list = ready_list
        self.room_id = room_id
        msg = create_rpc_data({"func_name": "RoomCreateSuccess"})
        for player_uid, player_conn in conn_list.items():
            ready_list[player_uid] = False
            room_index[player_uid] = room_id
            send(player_conn, const.FUNC_NAME_TO_OPCODE["rpc_command"], msg)

    def get_ready(self, player_uid):
        self.ready_list[player_uid] = True

        is_all_ready = True
        for _, is_ready in self.ready_list.items():
            if not is_ready:
                is_all_ready = False
                break

        if is_all_ready:
            msg = create_rpc_data({"func_name": "AllPlayerGameReady"})
            for player_uid, player_conn in self.conn_list.items():
                send(player_conn, const.FUNC_NAME_TO_OPCODE["rpc_command"], msg)


    def game_end(self):
        for player_uid, player_conn in self.conn_list.items():
            del room_index[player_uid]


    def broadcast_action_data(self):
        frame_data_str = self.frame_data.SerializeToString()
        for player_uid, player_conn in self.conn_list.items():
            send(player_conn, const.FUNC_NAME_TO_OPCODE["action_data"], frame_data_str)
        frame = self.frame_data.frame
        self.frame_data = tps_message_pb2.SC_Fight_Message()
        self.frame_data.frame = frame + 1

    def get_action_data(self, action_data):
        action_data_copy = self.frame_data.action_list.add()
        action_data_copy.action_id = action_data.action_id
        action_data_copy.action_type = action_data.action_type
        action_data_copy.player_id = action_data.player_id
        action_data_copy.action_param = action_data.action_param
        self.order_list.append(action_data)
        self.broadcast_action_data()