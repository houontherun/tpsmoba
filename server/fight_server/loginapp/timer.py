import time
from random import randint
timer_function_dict = {}
timer_tag_list = []
timer_id_current = 0

def insert_time_tag(last_trigger_time, timer_id, callback):
    index = len(timer_tag_list)
    for i, val in enumerate(timer_tag_list):
        if last_trigger_time > val["time"]:
            index = i
            break

    timer_tag_list.insert(index, {"time":last_trigger_time, "id":timer_id, "callback":callback})
    return index

def create_timer(callback, interval, times_left, param):
    global timer_id_current
    current_time = time.time()
    last_trigger_time = current_time + interval
    index = insert_time_tag(last_trigger_time, timer_id_current, callback)
    timer_function_dict[timer_id_current] = index
    timer_id_current += 1

def remove_timer(timer_id):
    if timer_function_dict.has_key(timer_id):
        tag_index = timer_function_dict[timer_id]
        timer_tag_list.pop(tag_index)

def timer_loop():
    current_time = time.time()

    for i in range(len(timer_tag_list), 0, -1):
        if timer_tag_list[i]["time"] >= current_time:
            break
        else:
            timer_data = timer_tag_list.pop()
            del timer_function_dict[timer_data["id"]]
            timer_data["callback"]()

if __name__ == '__main__':
    for i in range(1, 100):
        last_trigger_time = randint(1, 1000)
        insert_time_tag(last_trigger_time, i, "nothing")
    print timer_tag_list

    for i in range(len(timer_tag_list) - 1):
        if timer_tag_list[i]["time"] < timer_tag_list[i + 1]["time"]:
            print "error ", i