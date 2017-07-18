import time
from random import randint
timer_function_dict = {}
timer_tag_list = []
timer_id_current = 0

def insert_time_tag(last_trigger_time, timer_id):
    index = len(timer_tag_list)
    for i, val in enumerate(timer_tag_list):
        if last_trigger_time > val["time"]:
            index = i
            break

    timer_tag_list.insert(index, {"time":last_trigger_time, "id":timer_id})

def create_timer(callback, interval, times_left):
    global timer_id_current
    timer_id_current += 1
    current_time = time.time()
    last_trigger_time = current_time + interval
    insert_time_tag(last_trigger_time, timer_id_current)
    timer_function_dict[timer_id_current] = {"interval":interval, "id":timer_id_current, "callback":callback, "times_left":times_left}
    return timer_id_current

def remove_timer(timer_id):
    if timer_function_dict.has_key(timer_id):
        for i, val in enumerate(timer_tag_list):
            if timer_id == val["id"]:
                timer_tag_list.pop(i)
                break
        del timer_function_dict[timer_id]

def timer_loop():
    current_time = time.time()

    for i in range(len(timer_tag_list) - 1, -1, -1):
        last_trigger_time = timer_tag_list[i]["time"]
        if last_trigger_time >= current_time:
            break
        else:
            tag_data = timer_tag_list.pop()
            timer_id = tag_data["id"]
            timer_data = timer_function_dict[timer_id]
            timer_data["callback"]()

            if timer_data["times_left"] == -1:
                insert_time_tag(last_trigger_time + timer_data["interval"], timer_id)
            elif timer_data["times_left"] > 1:
                timer_data["times_left"] -= 1
                insert_time_tag(last_trigger_time + timer_data["interval"], timer_id)
            elif timer_data["times_left"] == 1:
                timer_data["times_left"] -= 1
                del timer_function_dict[timer_data["id"]]
            else:
                print "timer_loop error", timer_data["times_left"]


if __name__ == '__main__':
    for i in range(1, 100):
        last_trigger_time = randint(1, 1000)
        insert_time_tag(last_trigger_time, i, "nothing")
    print timer_tag_list

    for i in range(len(timer_tag_list) - 1):
        if timer_tag_list[i]["time"] < timer_tag_list[i + 1]["time"]:
            print "error ", i