# -*- coding: utf-8 -*-

import tps_message_pb2
import socket
import time
import sys
import struct
import msgpack
import rpc_func
from const import *

#SERVER_IP = "172.17.16.146"
#SERVER_IP = "172.17.17.94"
SERVER_IP = "127.0.0.1"
HOST_PORT = 1234
HEAD_SIZE_OFFSET = 4

state = "connect"
client_index = 1


def get_msg_from_buf(buf):
    #print sys._getframe().f_code.co_name  # 当前函数名
    p_length = 0
    total_len = len(buf)
    #print "total_len", total_len
    msg_list = []
    while p_length < total_len:
        last_buf = buf[p_length:]
        size, serial_number, src, dst, module, action = struct.unpack("<H3B2H", last_buf[:9])
        p_length += size
        p_length += 2
        #print "p_length", p_length
        msg_list.append(last_buf[9:p_length])

    #print msg_list
    return msg_list

n = 0
def sc_lua_message():
    global state
    global n
    print sys._getframe().f_code.co_name  # 当前函数名
    msg = recv_msg()
    #print rst_str
    if msg == "" or msg == None:
        return

    #print "recv len", len(rst_str)
    print msg
    action = msg["action"]
    data = msg["data"]
    if action == 1:
        rst = tps_message_pb2.SC_Fight_Message()
        rst.ParseFromString(data)
        if n < 50:
            n += 1
            state = "fight"
            time.sleep(0.1)
        else:
            state = "game_end"
    elif action == 2:
        rst = tps_message_pb2.SC_RPC_RunRequest()
        rst.ParseFromString(data)
        msgpack_data = msgpack.unpackb(rst.parameters)
        print msgpack_data
        if rst.opcode == 1002:
            if msgpack_data["func_name"] == "NewConnectionRet":
                pass
            elif msgpack_data["func_name"] == "RoomCreateSuccess":
                state = "ready"
            elif msgpack_data["func_name"] == "AllPlayerGameReady":
                state = "fight"

def encrypt(data):
    data_length = len(data)
    rst = bytearray(data_length)

    offset = HEAD_SIZE_OFFSET
    for i in range(0, offset):
        rst[i] = ord(data[i])
    for i in range(offset, data_length - 1):
        rst[i] = ord(data[i]) ^ ord(data[i + 1])
    rst[data_length - 1] = ord(data[data_length - 1]) ^ 58

    '''print "encrypt",
    for c in rst:
        print c,
    print "encrypt end"'''
    #print "send len ", data_length
    return rst


def send_msg(action, data):
    global serial_number

    #add head
    head_size = 6
    size = head_size + len(data)
    server_type = 0
    head = struct.pack("<I3H", size, serial_number, server_type, action)

    '''print "head",
    for c in head:
        print ord(c),
    print "head end"
    print "data",
    for c in data:
        print ord(c),
    print "data end"'''

    client.send(encrypt(head + data))

    serial_number += 1
    if serial_number > 255:
        serial_number = 0

msg_list = []
def recv_msg():
    global msg_list
    if len(msg_list) > 0:
        return msg_list.pop(0)

    buf = ""
    p_length = 0
    while True:
        buf += client.recv(1024)

        msg_len = len(buf)
        #print "total_len", msg_len

        while p_length < msg_len:
            head_offset = 10
            last_buf = buf[p_length:]
            if len(last_buf) < head_offset:
                break
            size, serial_number, server_type, action = struct.unpack("<I3H", last_buf[:head_offset])
            size += HEAD_SIZE_OFFSET
            p_length += size
            #print "p_length ", p_length
            #print "size ", size
            if p_length <= msg_len:
                msg_list.append({"data":last_buf[head_offset:size], "action":action})
                #print msg_list
                #print "len(msg_list) ", len(msg_list)
            else:
                p_length -= size
                break

        if p_length == msg_len:
            break

    if len(msg_list) > 0:
        return msg_list.pop(0)


def state_act():
    global state
    print "c" + str(client_index),
    if state == "rst_msg":
        sc_lua_message()
    else:
        print state
        msg = ""
        action = 2

        if state == "fight":
            action = 1
            msg = tps_message_pb2.CS_Fight_Message()
            msg.frame = 1
            action_data = msg.action_data
            action_data.action_id = 1
            action_data.action_type = 2
            action_data.player_id = "fasdf"
            action_data.action_param = "zzzzzzzzz"
        else:
            msg = tps_message_pb2.CS_RPC_RunRequest()
            msg.opcode = 1001
            if state == "connect":
                msg.parameters = msgpack.packb({'func_name': 'on_new_connection'})
            elif state == "ready":
                msg.parameters = msgpack.packb({'func_name': 'on_game_ready'})
            elif state == "game_end":
                msg.parameters = msgpack.packb({'func_name': 'on_game_end'})

            state = "rst_msg"

        msg_str = msg.SerializeToString()
        send_msg(action, msg_str)

        print ""


if __name__ == "__main__":
    args = sys.argv
    if len(args) > 1:
        client_index = int(args[1])
    print "client_index", client_index

    loop_times = 1
    while loop_times > 0:
        loop_times -= 1
        client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        serial_number = 0

        client.connect((SERVER_IP, HOST_PORT))
        #client.connect(("127.0.0.1", HOST_PORT))
        #client.setblocking(1)
        #client.settimeout(2)

        while True:
            if state == "end":
                client.close()
                client = None
                break
            #time.sleep(0.1)
            state_act()
