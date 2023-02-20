import pandas as pd
import numpy as np
from img_io import *

if __name__=="__main__":
    data=pd.read_csv("Brain_t_game_record.csv")
    data=data.fillna(" ")
    s = set(data["SWCId"])
    data=np.array(data)
    swc_user=[]
    swc_times=[]
    for i in s:
        swc_times.append([i,0])
        swc_user.append([i])
    for line in data:
        if line[3]==' ' and line[4]==' ' and line[5]==' ':
            continue
        for st in swc_times:
            if st[0]==line[2]:
                st[1]+=1
                break
        for su in swc_user:
            if su[0]==line[2]:
                su.append(line[1])
                break
    print(swc_user)
    print(swc_times)
    swc_users=[]
    for list in swc_user:
        s=set(list)
        swc_users.append([list[0],len(s)-1])
    firstline1 = "ImageID, playercount"         #single record
    swc_users=np.array(swc_users)
    Writecsv(swc_users,"playercount.csv",firstline1)

    firstline2 = "ImageID, playtimes"  # single record
    swc_times = np.array(swc_times)
    Writecsv(swc_times, "playtimes.csv", firstline2)