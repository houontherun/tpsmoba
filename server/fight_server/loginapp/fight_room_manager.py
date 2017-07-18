from network import send
import uuid
import const
import fight_room
from network import send, create_rpc_data

ROOM_PLAYER_NUMBER = 1

room_list = {}
waiting_list = {}

def on_new_connection(conn, input):
    global waiting_list
    uid = conn.uid
    if len(waiting_list) < ROOM_PLAYER_NUMBER:
        waiting_list[uid] = conn

    msg = create_rpc_data({"func_name": "NewConnectionRet"})
    send(conn, const.FUNC_NAME_TO_OPCODE["rpc_command"], msg)

    if len(waiting_list) == ROOM_PLAYER_NUMBER:
        room_id = uuid.uuid1()
        room_list[room_id] = fight_room.fight_room(room_id, waiting_list)
        waiting_list = {}


def on_game_end(conn, input):
    uid = conn.uid
    room_id = fight_room.get_room_id_by_player_id(uid)
    if room_id != None and room_list[room_id] != None:
        room_list[room_id].game_end()
        del room_list[room_id]

def on_game_ready(conn, input):
    uid = conn.uid
    room_id = fight_room.get_room_id_by_player_id(uid)
    if room_id != None and room_list[room_id] != None:
        room_list[room_id].get_ready(uid)


def transport_fight_msg(conn, msg):
    uid = conn.uid
    room_id = fight_room.get_room_id_by_player_id(uid)
    if room_id != None and room_list[room_id] != None:
        room_list[room_id].get_action_data(msg)
