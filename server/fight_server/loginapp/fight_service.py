# -*- coding: utf-8 -*-
import json
import redis
import hashlib
from config import config
import const
import net_decode
from network import send

class fight_service(object):
    """ 屏蔽sdk细节"""

    def __init__(self):
        pass

    def init(self):
        ip = config["redis"]["address"]
        port = config["redis"]["port"]
        self.init_db(ip, port)

    def init_db(self, ip, port, enable_sentinel=False):
        if enable_sentinel:
            #TODO sentinel 模式
            pass
        else:
            self.redisconn = redis.StrictRedis(ip, port, 0)

    def process(self, conn, opcode, data):
        print "opcode:", opcode
        print "data:", data
        func_name = const.OPCODE_TO_FUNC_NAME[opcode]
        if func_name == None:
            print "error opcode:", opcode
            assert False
        func = getattr(net_decode, func_name)
        if func == None:
            print "error no func_name:", func_name
            assert False
        func(conn, data)
        # handler = self.handles[opcode]
        # if handler:
        #     handler(conn, data)
