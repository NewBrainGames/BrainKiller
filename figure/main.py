import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from img_io import *
import glob as glob
import math
from figuremain import *
from modifyswc import modifyswc

currentmode=""

def process_csv(recordpath):
    data=pd.read_csv(recordpath)
    data=data.fillna(" ")
    s = set(data["SWCId"])
    data=np.array(data)

    record=[]
    swcpoints=[]
    for swcid in s:
        record.append([swcid,0])
        swcpoint=[]
        swcpoint.append([swcid,-1])
        for d in data:
            if(d[2]==swcid) and d[1]!="hyn":
                record[len(record)-1][1]+=1
                points=[]
                if currentmode=="correct":
                    points=d[3].split(":")[-1].split(",") #correct
                    #print(points)
                elif currentmode=="wrong":
                    points=d[4].split(",") #wrong
                elif currentmode=="miss":
                    points = d[5].split(",")  # miss
                # print(points)
                for p in points:
                    if p !=" " and p!="":
                        flag=False
                        for sp in swcpoint:
                            if sp[0]==p:
                                sp[1]+=1
                                flag=True
                        if flag==False:
                            swcpoint.append([p,1])

        #print(swcpoint)
        swcpoints.append(swcpoint)


    # Alldata=0
    # Truedata=0
    # result=[]
    # for points in swcpoints:
    #     if len(points)<=1:
    #         continue
    #     alldata=0
    #     truedata=0
    #     count=0
    #     for point in points:
    #         if point[1]==-1:
    #             continue
    #         count+=point[1]
    #         alldata+=1
    #     Alldata+=alldata
    #     for point in points:
    #         if point[1]==-1:
    #             continue
    #         point[1]=point[1]/count
    #
    #     averageratio=1/(len(points)-1)
    #     for point in points:
    #         if point[1]==-1:
    #             continue
    #         if point[1]>=averageratio:
    #             truedata+=1
    #     Truedata+=truedata
    #     result.append([points[0][0],truedata/alldata])


    # print(record)
    # print(swcpoints)
    return swcpoints

def process_csv2(recordpath):
    data = pd.read_csv(recordpath)
    data = data.fillna(" ")
    s = set(data["SWCId"])
    data = np.array(data)

    record = []
    swcpoints = []
    for swcid in s:
        record.append([swcid, 0])
        swcpoint = []
        swcpoint.append([swcid, -1])
        for d in data:
            if (d[2] == swcid) and d[1] != "hyn":
                record[len(record) - 1][1] += 1
                correctpoints=d[3].split(":")[-1].split(",")
                wrongpoints=d[4].split(",")
                for p in correctpoints:
                    if p != " " and p != "":
                        flag = False
                        for sp in swcpoint:
                            if sp[0] == p:
                                sp[1] += 1
                                flag = True
                        if flag == False:
                            swcpoint.append([p, 1])
                for p in wrongpoints:
                    if p != " " and p != "":
                        flag = False
                        for sp in swcpoint:
                            if sp[0] == p:
                                sp[1] -= 1
                                flag = True
                        if flag == False:
                            swcpoint.append([p, -1])
        swcpoints.append(swcpoint)
    return swcpoints
    # print(Alldata)
    # print(Truedata)
    # print(result)
    # result=np.array(result)
    # # sorted(result, key=lambda x: x[1])
    #
    # #result=np.sort(result,axis=0)
    # print(result)
    # plt.plot(range(len(result[:,0])),result[:,1])
    #
    # plt.show()
def process_csv3(recordpath):
    data=pd.read_csv(recordpath)
    data=data.fillna(" ")
    s = set(data["SWCId"])
    username = set(data["UserName"])
    data=np.array(data)
    swcpointscorrect=[]
    swcpointswrong = []
    swcpointsmiss = []
    swcpointsuser = []
    for swcid in s:
        swcpointcorrect=[]
        swcpointwrong = []
        swcpointmiss = []
        swcpointuser=[]
        swcpointcorrect.append([swcid,-1])
        swcpointwrong.append([swcid, -2])
        swcpointmiss.append([swcid, -3])
        swcpointuser.append([swcid,-4])
        for uname in username:
            correctset = set()
            wrongset = set()
            missset = set()
            for d in data:
                if d[1]==uname and d[2]==swcid:
                    pointscorrect = d[3].split(":")[-1].split(",")  # correct
                    pointswrong = d[4].split(",")  # wrong
                    pointsmiss = d[5].split(",")  # miss
                    for item in pointscorrect:
                        if item != " " and item != "":
                            correctset.add(item)
                    for item in pointswrong:
                        if item != " " and item != "":
                            wrongset.add(item)
                    for item in pointsmiss:
                        if item != " " and item != "":
                            missset.add(item)
            swcpointuser.append([uname,[correctset,wrongset,missset]])
        swcpointsuser.append(swcpointuser)
        for d in data:
            if(d[2]==swcid):# and d[1]!="hyn":
                pointscorrect=d[3].split(":")[-1].split(",") #correct
                pointswrong=d[4].split(",") #wrong
                pointsmiss = d[5].split(",")  # miss
                #print(pointscorrect,pointswrong,pointsmiss)
                for p in pointscorrect:
                    if p !=" " and p!="":
                        flag=False
                        for sp in swcpointcorrect:
                            if sp[0]==p:
                                sp[1]+=1
                                flag=True
                        if flag==False:
                            swcpointcorrect.append([p,1])
                for p in pointswrong:
                    if p !=" " and p!="":
                        flag=False
                        for sp in swcpointwrong:
                            if sp[0]==p:
                                sp[1]+=1
                                flag=True
                        if flag==False:
                            swcpointwrong.append([p,1])
                for p in pointsmiss:
                    if p !=" " and p!="":
                        flag=False
                        for sp in swcpointmiss:
                            if sp[0]==p:
                                sp[1]+=1
                                flag=True
                        if flag==False:
                            swcpointmiss.append([p,1])
        swcpointscorrect.append(swcpointcorrect)
        swcpointswrong.append(swcpointwrong)
        swcpointsmiss.append(swcpointmiss)
    return swcpointscorrect,swcpointswrong,swcpointsmiss,swcpointsuser

def FindBranchingPoints(swc):
    nums=np.zeros(swc.shape[0]+1)
    flag=np.zeros(swc.shape[0]+1)
    for i in range(swc.shape[0]):
        if swc[i][6]!=-1:
            nums[int(swc[i][6])]+=1
            # print(int(swc[i][6]),nums[int(swc[i][6])])
            if nums[int(swc[i][6])]==2:
                flag[int(swc[i][6])]=1
    result=[]
    for i in range(flag.shape[0]):
        if flag[i]==1:
            result.append(i)
    return result

def getCorrectBP(bpset,resultlist):
    res=[]
    for swcpoints in resultlist:
        flag=False
        for point in swcpoints:
            if point[0][0]=='I':
                flag=True
                break
            if point[1]==-1:
                continue
            #print(bpset)
            # print(swcpoints)
            # print(point[0])
            if int(point[0]) in bpset:
                res.append(int(point[0]))
        if flag==True:
            continue
    # print(res)

def distancemarkerswc(marker,swc):
    return math.sqrt((marker[0]-swc[2])**2+(marker[1]-swc[3])**2+(marker[2]-swc[4])**2)

def distanceswcswc(swc1,swc2):
    return math.sqrt((swc1[2]-swc2[2])**2+(swc1[3]-swc2[3])**2+(swc1[4]-swc2[4])**2)

def marker2goldenstandard(markerpath,swcpath,lamda=5):
    goldenstandrad=[]
    swc=Readswc(swcpath)
    # print(swc)
    # print(swcpath)
    swcbp=FindBranchingPoints(swc)
    markers=ReadMarker(markerpath)
    # print(markerpath)
    # print(swcbp)
    for marker in markers:
        mmin=65536
        index=-1
        for i in swcbp:
            if distancemarkerswc(marker,swc[i-1])<mmin:
                mmin=distancemarkerswc(marker,swc[i-1])
                index=i
        if mmin<lamda:
            goldenstandrad.append(index)
    # print(set(goldenstandrad))
    return set(goldenstandrad),len(swcbp)
    # print(marker)
def miss2correct(markerpath,swcpath,index,lamda=5):
    swc=Readswc(swcpath)
    # print(swc)
    markers=ReadMarker(markerpath)
    mmin = 65536
    for marker in markers:
        if distancemarkerswc(marker,swc[index-1])<mmin:
            mmin=distancemarkerswc(marker,swc[index-1])
        if mmin<lamda:
            return True
    return False

def summary(resultlist,namelist):
    # print(namelist)
    first=[]
    for swcpoints in resultlist:
        if swcpoints[0][0][0]!='I':
            continue
        # print(swcpoints)
        list=[]
        for name in namelist:
            if name[:13]==swcpoints[0][0][:13]:
                # print(name)
                list.append([name,-1])
                for swcpoint in swcpoints:
                    if swcpoint[1]==-1:
                        continue
                    list.append(swcpoint)
                break
        if len(list)>0:
            first.append(list)

    second=[]
    for name in namelist:
        list=[]
        list.append([name,-1])
        for items in first:
            if items[0][0]==name:
                for it in items:
                    # print(it)
                    if it[1]==-1:
                        continue
                    for i in list:
                        if i[0]!=it[0]:
                            list.append(it)
                            break
                        else:
                            i[1]+=it[1]
        # print(list)
        mmax=-2
        for item in list:
            if item[1]==-1:
                continue
            if item[1]>mmax:
                mmax=item[1]
        newlist=[]
        newlist.append(list[0])
        for item in list:
            if item[1]==-1:
                continue
            # if item[1] > 0:             #correct>wrong
            #     newlist.append(item)
            if item[1]>mmax*0.5:
                newlist.append(item)
        # print(newlist)
        #if len(newlist)>mmax*0.5:
        second.append(newlist)

    return second

def summary2(resultlist,namelist):
    # print(namelist)
    first=[]
    for swcpoints in resultlist:
        if swcpoints[0][0][0]!='I':
            continue
        # print(swcpoints)
        list=[]
        for name in namelist:
            if name[:13]==swcpoints[0][0][:13]:
                # print(name)
                list.append([name,-1])
                for swcpoint in swcpoints:
                    if swcpoint[1]==-1:
                        continue
                    list.append(swcpoint)
                break
        if len(list)>0:
            first.append(list)
    # print(first)
    second=[]
    for name in namelist:
        list=[]
        list.append([name,-1])
        for items in first:
            if items[0][0]==name:
                for it in items:
                    # print(it)
                    if it[1]==-1:
                        continue
                    for i in list:
                        if i[0]!=it[0]:
                            list.append(it)
                            break
                        else:
                            i[1]+=it[1]
        # print(list)
        mmax=-2
        for item in list:
            if item[1]==-1:
                continue
            if item[1]>mmax:
                mmax=item[1]
        newlist=[]
        newlist.append(list[0])
        for item in list:
            if item[1]==-1:
                continue
            # if item[1] > 0:             #correct>wrong
            #     newlist.append(item)
            if item[1]<0:
                newlist.append(item)
        print(newlist)
        if len(newlist)>1:
            second.append(newlist)

    return second

def summaryindex(summary):
    summindex=[]
    for swc in summary:
        list=[]
        list.append(swc[0][0])
        s=set()
        for point in swc:
            if point[1]==-1:
                continue
            # print(point)
            s.add(point[0])
        # print(s)
        for i in s:
            list.append(int(i))
        # if len(list)!=1:
        summindex.append(list)
    # print(summindex)
    return summindex

def CalRecordRatio(record_path,goldenstandard,name):
    data = pd.read_csv(record_path)
    data = data.fillna(" ")
    #print(data)
    data = np.array(data)
    #print(goldenstandard)
    ratio=[]
    for datai in data:
        # print(datai[2][:13])
        # print(name[:13])
        if datai[2][:13]==name[:13]:
            # print(datai[3])
            points=datai[3].split(",")
            #print(points)
            count=0
            total=len(points)-1
            for point in points:
                if point==' ':
                    continue
                if int(point) in goldenstandard:
                    #print(point)
                    count+=1
            # print(count)
            # print(total)
            if total!=0:
                line=[]
                line.append(datai[1])
                line.append(name)
                line.append(str(count/total))
                # print(line)
                ratio.append(line)
    #print(ratio)
    return ratio


def dendrite():
    # analyzetype=input("Please input the data type.(correct,wrong,miss)")
    modes = ["correct", "wrong", "miss"]

    #modes=["correct"]      #single record

    for mode in modes:
        global currentmode
        currentmode = mode

        path1 = "Brain_t_game_record_after0201.csv"
        resultlist = process_csv(path1)
        #resultlist = process_csv2(path1)
        # print(len(resultlist))

        # swcpath="18454_00002.swc"
        # swc=Readswc(swcpath)
        # bpset=FindGoldenStandardBranchingPoints(swc)
        # getCorrectBP(bpset,resultlist)

        goldenstandradpath = r"E:\csz\csz\csz\*"
        swcpath = r"E:\csz\dendriteSWC\*.swc"
        markerfiles = glob.glob(goldenstandradpath)
        # print(markerfiles)
        namelist = []
        for markerfile in markerfiles:
            names = markerfile.split('\\')[-1].split('.')
            name = ""
            for i in range(len(names) - 2):
                name += names[i] + '.'
            # print(name)
            namelist.append(name)

        summ = summary(resultlist, namelist)
        #summ = summary2(resultlist, namelist)
        # print(summ)
        summindex = summaryindex(summ)
        # print(summindex)
        csv = []
        totalgold=[]
        totalinitswcbp=[]
        for name in namelist:
            swcname = r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\dendriteSWC" + "\\" + name + "swc_sorted.swc"
            markername = r"E:\csz\csz\csz" + "\\" + name + "v3draw.marker"
            goldenstandard, lenswcbp = marker2goldenstandard(markername, swcname)
            totalgold.append([name,goldenstandard])
            totalinitswcbp.append([name,FindBranchingPoints(Readswc(swcname))])
            # ratio=CalRecordRatio(path1,goldenstandard,name)     #single record
            # for line in ratio:
            #     csv.append(line)


            data = []
            for i in summindex:
                if name == i[0]:
                    data = i
            #print(data)
            correctbp = 0
            if currentmode != "miss":
                for i in range(len(data)):  # wrong,correct
                    if i == 0:
                        continue
                    # print(data)

                    if data[i] in goldenstandard:
                        correctbp += 1
            else:
                # print(correctbp)
                for i in range(len(data)):  # miss
                    if i == 0:
                        continue
                    if miss2correct(markername, swcname, data[i], 15):
                        correctbp += 1

            csvline = [name, len(goldenstandard), lenswcbp, correctbp, len(data) - 1]
            csv.append(csvline)
        #print(resultlist)
        # print(summ)
        # print(summindex)
        # print(totalgold)
        correctrate(totalinitswcbp,summindex,totalgold,namelist,currentmode)
        firstline = "ImageID, count_of_goldstandard, count_of_bps_in_swcfile, correctbps_in_players_file, count_of_records_about_this_img(swc)"
        csv = np.array(csv)
        Writecsv(csv, currentmode + "bp.csv", firstline)


        # firstline = "userid, img_name, accuracy_rate"         #single record
        # csv=np.array(csv)
        # # print(csv)
        # Writecsv(csv,"accuracy_rate.csv",firstline)
def timeserilizeddendrite():

    path3 = "Brain_t_game_record_after0201.csv"
    resultlist = process_csv3(path3)
    #print(resultlist[3])
    goldenstandradpath = r"E:\csz\csz\csz\*"
    # swcpath = r"E:\csz\dendriteSWC\*.swc"
    markerfiles = glob.glob(goldenstandradpath)
    namelist = []
    for markerfile in markerfiles:
        names = markerfile.split('\\')[-1].split('.')
        name = ""
        for i in range(len(names) - 2):
            name += names[i] + '.'
        namelist.append(name)
    #
    # summ = summary(resultlist, namelist)
    # summindex = summaryindex(summ)
    totalgold=[]
    # totalinitswcbp=[]
    for name in namelist:
        swcname = r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\dendriteSWC" + "\\" + name + "swc_sorted.swc"
        markername = r"E:\csz\csz\csz" + "\\" + name + "v3draw.marker"
        goldenstandard, lenswcbp = marker2goldenstandard(markername, swcname)
        totalgold.append([name,goldenstandard])
    #print(totalgold)
    #print(FindExpertPlayer(resultlist[3],totalgold,-1))
    # print(resultlist[0])
    # print(resultlist[1])
    # print(resultlist[2])
    imgTimeseries(resultlist[3],totalgold,expertscore=5,mode="dendrite")



    # savedir=r"D:\A_result\game\gameswc"
    # for name in namelist:
    #     recordcorrect=[]
    #     recordwrong = []
    #     recordmiss = []
    #     gold=[]
    #     for item in resultlist[0]:
    #         if item[0][0][:13]==name[:13]:
    #             recordcorrect=item
    #             break
    #     for item in resultlist[1]:
    #         if item[0][0][:13]==name[:13]:
    #             recordwrong=item
    #             break
    #     for item in resultlist[2]:
    #         if item[0][0][:13]==name[:13]:
    #             recordmiss=item
    #             break
    #     record=[recordcorrect,recordwrong,recordmiss]
    #     for item in totalgold:
    #         if item[0]==name:
    #             gold=item
    #             break
    #     modifyswc(name,record,gold,savedir)



def FindPointinSwc(coor,gold):
    bpindex=FindBranchingPoints(gold)
    #print(bpindex)
    maxbias=65536**2
    index=-2
    for bp in bpindex:
        swcline=gold[bp-1]
        bias=(coor[0]-swcline[2])**2+(coor[1]-swcline[3])**2+(coor[2]-swcline[4])**2
        if bias<maxbias:
            maxbias=bias
            index=bp
    return index,maxbias


def axon():
    swcpath=r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\ServerSWCFiles"
    imgpath=r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\AxonPbdImages\*.v3dpbd"
    goldpath="18454_00002.swc"
    gold=Readswc(goldpath)
    modes = ["correct","wrong","miss"]
    #s=['10274_14190_5344', '10141_14125_5325', '10523_14450_5273', '10440_14442_6223', '10374_14335_5290', '10329_14098_6665', '10372_13907_6742', '10522_14288_6565', '10400_14386_6393', '10448_14428_6254', '10357_14232_6623', '10438_14362_5296', '10422_14407_5307', '10510_14338_5325']
    for mode in modes:
        global currentmode
        currentmode= mode
        path2 = "Brain_t_game_record(4).csv"
        resultlist = process_csv(path2)
        axonswc=[]
        for result in resultlist:
            if len(result)==1:
                continue
            if result[0][0][:3]=="Img":
                continue
            axonswc.append(result)
        # print(len(axonswc))
        totalpoints=[]
        axoncsv=[]


        # correctimgname=[]             #Preprocess
        # for result in axonswc:
        #     if len(result)==1:
        #         continue
        #     if result[0][0][:3]=="Img":
        #         continue
        #     #print(result)
        #     imglist=glob.glob(imgpath)
        #     swcnamelist=[]
        #     for img in imglist:
        #         name=img.split("\\")[-1].split(".")[0]
        #         swcnamelist.append(name)
        #     # print(swcnamelist)
        #     maxbp=0
        #     correctblock=""
        #     for swcname in swcnamelist:
        #         singleswcpath=swcpath+"\\"+swcname+".swc"
        #         singleswc=Readswc(singleswcpath)
        #         singleswcbps=FindBranchingPoints(singleswc)
        #         #print(singleswcbps)
        #         bpcnt=0
        #         for bp in result:
        #             if bp[1]==-1:
        #                 continue
        #             if int(bp[0]) in singleswcbps:
        #                 bpcnt+=1
        #         if bpcnt>maxbp:
        #             maxbp=bpcnt
        #             correctblock=swcname
        #     if correctblock!=result[0][0]:
        #         cflag=False
        #         for correctimg in correctimgname:
        #             if result[0][0]==correctimg[0]:
        #                 cflag=True
        #                 break
        #         if cflag==False:
        #             correctimgname.append([result[0][0],correctblock])
        # print(correctimgname)
        # record=pd.read_csv(path2)
        # for correctname in correctimgname:
        #     record.replace(correctname[0],correctname[1],inplace=True)
        #     print(correctname)
        # record.to_csv(path2,index=False)


        for result in axonswc:
            x,y,z=result[0][0].split("_")
            # print(result[0][0])
            swc=Readswc( swcpath+"\\"+result[0][0]+".swc")
            maxn=-1
            for p in result:
                if p[1]==-1:
                    continue
                if p[1]>maxn:
                    maxn=p[1]
            resultnew=[]
            resultnew.append(result[0])
            for p in result:
                if p[1]==-1:
                    continue
                if p[1]>=maxn*0.5:
                    resultnew.append(p)
            pcount=0
            print(resultnew)
            for p in resultnew:
                # print(p)
                if p[1]==-1:
                    continue
                if int(p[0])>swc.shape[0]:
                    print(resultnew[0][0])
                    # k+=1
                    # print(int(p[0]))
                    # print(swc[int(p[0])-1])
                globalcoordinate=[float(x)+swc[int(p[0])-1][2]-129,float(y)+swc[int(p[0])-1][3]-129,float(z)+swc[int(p[0])-1][4]-129]
                    # print(globalcoordinate)
                bpindex,maxbias=FindPointinSwc(globalcoordinate,gold)
                print(bpindex,maxbias)
                if maxbias<5:
                    pcount+=1
                    flag=False
                    for tp in totalpoints:
                        if tp[0]==bpindex:
                            tp[1]+=1
                            flag=True
                            break
                    if flag==False:
                        newbp=[bpindex,1]
                        totalpoints.append(newbp)
                # else:
                #     s.add(resultnew[0][0])
            if mode=="correct":
                rawbpindex=FindBranchingPoints(swc)
                rawpcount=0
                rawtotalcount=len(rawbpindex)
                for index in rawbpindex:
                    rawglobalcoordinate = [float(x) + swc[int(index) - 1][2] - 129, float(y) + swc[int(index) - 1][3] - 129,
                                        float(z) + swc[int(index) - 1][4] - 129]
                    rawbpindex, rawmaxbias = FindPointinSwc(rawglobalcoordinate, gold)
                    if rawmaxbias < 5:
                        rawpcount += 1
                axoncsv.append([result[0][0],rawpcount,rawtotalcount,pcount,len(result)-1])
            else:
                axoncsv.append([result[0][0],pcount,len(result)-1])
        # print(s)
        print(totalpoints)
        print(axoncsv)
        if mode=="correct":
            axoncsv=np.array(axoncsv)
            firstline = "ImageId, rawcorrectbp, rawtotalbp, correctbp, totalbp"  # single record
            Writecsv(axoncsv,"axon_correct.csv",firstline)
        elif mode=="wrong":
            axoncsv=np.array(axoncsv)
            firstline = "ImageId, wrongbp, totalbp"  # single record
            Writecsv(axoncsv,"axon_wrong.csv",firstline)
        elif mode=="miss":
            axoncsv=np.array(axoncsv)
            firstline = "ImageId, missbp, totalbp"  # single record
            Writecsv(axoncsv,"axon_miss.csv",firstline)


def timeserilizedaxon():

    path3 = "Brain_t_game_record_after0201.csv"
    resultlist = process_csv3(path3)
    goldenstandradpath = r"D:\A_result\swc_out"
    namelist = []
    for img in resultlist[3]:
        if img[0][0][:3]!="Img":
            namelist.append(img[0][0])
    gold=[]
    for name in namelist:
        swcpath=goldenstandradpath+"\\"+name+".swc"
        bps=FindBranchingPoints(Readswc(swcpath))
        gold.append([name,bps])
    print(gold)
    imgTimeseries(resultlist[3],gold,expertscore=0,mode="axon")
    # for markerfile in markerfiles:
    #     names = markerfile.split('\\')[-1].split('.')
    #     name = ""
    #     for i in range(len(names) - 2):
    #         name += names[i] + '.'
    #     namelist.append(name)
    # #
    # # summ = summary(resultlist, namelist)
    # # summindex = summaryindex(summ)
    # totalgold=[]
    # # totalinitswcbp=[]
    # for name in namelist:
    #     swcname = r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\dendriteSWC" + "\\" + name + "swc_sorted.swc"
    #     markername = r"E:\csz\csz\csz" + "\\" + name + "v3draw.marker"
    #     goldenstandard, lenswcbp = marker2goldenstandard(markername, swcname)
    #     totalgold.append([name,goldenstandard])
    #print(totalgold)
    #print(FindExpertPlayer(resultlist[3],totalgold,-1))
    # print(resultlist[0])
    # print(resultlist[1])
    # print(resultlist[2])
    # imgTimeseries(resultlist[3],totalgold,expertscore=5)
    # savedir=r"D:\A_result\game\gameswc"
    # for name in namelist:
    #     recordcorrect=[]
    #     recordwrong = []
    #     recordmiss = []
    #     gold=[]
    #     for item in resultlist[0]:
    #         if item[0][0][:13]==name[:13]:
    #             recordcorrect=item
    #             break
    #     for item in resultlist[1]:
    #         if item[0][0][:13]==name[:13]:
    #             recordwrong=item
    #             break
    #     for item in resultlist[2]:
    #         if item[0][0][:13]==name[:13]:
    #             recordmiss=item
    #             break
    #     record=[recordcorrect,recordwrong,recordmiss]
    #     for item in totalgold:
    #         if item[0]==name:
    #             gold=item
    #             break
    #     modifyswc(name,record,gold,savedir)



if __name__ == '__main__':
    #dendrite()
    # axon()
    timeserilizeddendrite()
    #timeserilizedaxon()