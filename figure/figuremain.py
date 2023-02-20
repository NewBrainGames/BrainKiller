import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
from img_io import *

def FindExpertPlayer(result,goldstandard,n,minscore=0):
    print("start finding expert!")
    experteachimg=[]
    for gold in goldstandard:
        Imgid=gold[0]
        standard=gold[1]
        recordrank=[]
        for img in result:
            # print(img[0][0])
            # print("goldname:",Imgid)
            # print(img)
            if img[0][0][:13]==Imgid[:13]:
                expertrate=[]
                for usrrecord in img:
                    if usrrecord[1]==-4:
                        continue
                    #the rule to choose the expert
                    score=0
                    for item in usrrecord[1][0]:
                        if int(item) in standard:
                            score+=1
                        else:
                            score-=1
                    for item in usrrecord[1][1]:
                        if int(item) in standard:
                            score-=0.5
                        else:
                            score+=2
                    for item in usrrecord[1][2]:
                        if int(item) in standard:
                            score+=3
                    expertrate.append([usrrecord[0],score])
                expertrate=sorted(expertrate,key=lambda x:x[1],reverse=True)
                if n>0:
                    for i in range(n):
                        if expertrate[i][1]<=minscore :
                            break
                        for usrrecord in img:
                            if usrrecord[1]==-4:
                                continue
                            if usrrecord[0]==expertrate[i][0]:
                                recordrank.append(usrrecord)
                                break
                else:
                    for i in range(len(expertrate)):
                        if expertrate[i][1]<=minscore:
                            break
                        for usrrecord in img:
                            if usrrecord[1]==-4:
                                continue
                            if usrrecord[0]==expertrate[i][0]:
                                recordrank.append(usrrecord)
                                break
        experteachimg.append([Imgid,recordrank])
    return experteachimg





def correctrate(totalinitswcbp,summindex,goldstandard,namelist,mode):                  #相较于原始金标准提升的比率
    print(mode)
    if mode!="correct":
        return
    print("correct_rate")
    result = {
        "img_id": [],
        "Original_Correct_Rate": [],
        "Player_Correct_Rate": [],
        "GoldStandard_Hit_Rate":[],
        "OMinusP": [],
        "correct":[]
    }
    for name in namelist:
        summi=[]
        goldi=[]
        swcbpi=[]
        for i in summindex:
            # print(i[0])
            if i[0]==name:
                summi=i
                break
        for i in goldstandard:
            if i[0]==name:
                goldi=i
                break
        for i in totalinitswcbp:
            if i[0]==name:
                swcbpi=i
        print("summi:",summi)
        print("goldi:",goldi)
        print("swcbp:", swcbpi)
        initcount=0
        playerhitcount=0
        for i in swcbpi[1]:
            if i in goldi[1]:
                initcount+=1
        for i in summi:
            if i==name:
                continue
            if i in goldi[1]:
                playerhitcount+=1
        print(initcount)
        print(playerhitcount)
        result["img_id"].append(name)
        result["Original_Correct_Rate"].append(initcount/len(swcbpi[1]))
        result["Player_Correct_Rate"].append(playerhitcount/(len(summi)-1))
        result["GoldStandard_Hit_Rate"].append(playerhitcount/initcount)
        print(playerhitcount/(len(summi)-1)-initcount/len(swcbpi[1]))
        result["OMinusP"].append(playerhitcount/(len(summi)-1)-initcount/len(swcbpi[1]))
        result["correct"].append(playerhitcount)
    finalData = pd.DataFrame(result)
    finalData = finalData.sort_values(by=['correct'], ascending=[True])  # 按玩家正确率递增排序
    plt.rcParams['font.sans-serif'] = 'Microsoft YaHei'
    tick_label = list(finalData.iterrows())
    x = np.arange(len(tick_label))
    y1 = finalData['Original_Correct_Rate'].values.tolist()  # 收入(剔除自己转入)
    y2 = finalData['Player_Correct_Rate'].values.tolist()  # 支出(剔除自己转入)
    y3 = finalData['GoldStandard_Hit_Rate'].values.tolist()  # 支出(剔除自己转入)
    fig, ax1 = plt.subplots()
    bar_with = 0.5  # 柱体宽度plt.figure(figsize = (12,6)) #画布大小
    ax1.bar(x, y1, width=bar_with,  # 柱体宽度
            align='center',  # x轴上的坐标与柱体对其的位置
            color='aqua', alpha=0.6,  # 柱体透明度
            label='before_game')
    ax1.bar(x, y2, width=bar_with,
            # bottom=y1,  # 柱体基线的y轴坐标
            align='center', color='orange', alpha=0.6, label='after_game')
    # ax2 = ax1.twinx()
    # ax2.plot(x,y3,c='aqua',label="difference")
    # ax2.set_ylim(0.4,1.1)
    plt.title('Dendrite Data Effective Rate', fontdict={'family': 'Times New Roman', 'size': 14})  # 设置x轴标题
    plt.ylim(0,1)
    # plt.xticks(x + bar_with / 2, tick_label, rotation=70)  # 设置x轴坐标
    # plt.xlabel('时间',fontsize = 8, verticalalignment = 'top', horizontalalignment ='right',rotation='horizontal')
    # plt.xlabel('时间',fontsize = 8, verticalalignment = 'bottom', horizontalalignment ='center')
    # 图例设在图形外面，控制坐标参数
    lines, labels = ax1.get_legend_handles_labels()
    # lines2, labels2 = ax2.get_legend_handles_labels()
    # lines += lines2
    # labels += labels2
    # ax1.legend(loc="upper left",bbox_to_anchor=(0, 1), ncol=1,prop = font_legend)
    font_legend = {'family': 'Times New Roman',
                   'weight': 'normal',
                   'size': 10,
                   }
    plt.legend(lines, labels, loc="upper left", bbox_to_anchor=(0, 1), ncol=1, prop=font_legend)
    plt.savefig('D:/A_result/gamefigure/Dendrite_Data_Effective_Rate.png', dpi=300, bbox_inches='tight')
    plt.show()



def wrongrate():                    #玩家击中的错误点是错误点的比率
    print("wrong_rate")

def missrate():                     #玩家击中的miss点是miss点的比率
    print("miss_rate")

def imghitrate():                   #单图被击中的正确点占总击中点的比率
    print("img_hit_rate")

def playerhitrate():                #单图玩家命中率的平均数
    print("player_hit_rate")

def imgTimeseries(result,goldstandard,n=-1,expertscore=0,mode="dendrite"):                #单图玩家命中率的平均数
    print("img time series")
    ExpertEachImg=FindExpertPlayer(result,goldstandard,n,expertscore)
    #print(ExpertEachImg)
    for img in ExpertEachImg:
        summary = {
            "img_id": [],
            "correct": [],
            "wrong": [],
            "miss": [],
            "totalcorrect": [],
            "rate": []
        }

        total=[]
        standard={}
        for gold in goldstandard:
            if gold[0]==img[0]:
                standard=gold[1]
                break
        if len(standard)>0:
            for i in range(len(img[1])):
                #print(i)
                index=len(img[1])-i-1
                usrrecord=img[1][index]
                correctn=0
                wrongn=0
                missn=0
                totaln=1e-8
                #print(usrrecord)
                for item in usrrecord[1][0]:
                    flag=False
                    for j in total:
                        if item==j[0]:
                            j[1]+=1
                            flag=True
                            break
                    if flag==False:
                        total.append([item,1])
                #print(total)
                for item in usrrecord[1][1]:
                    if int(item) in standard:
                        wrongn+=1
                for item in usrrecord[1][2]:
                    if int(item) in standard:
                        missn+=1
                mmax=-1
                for j in total:
                    if j[1]>mmax:
                        mmax=j[1]
                # for j in total:
                #     if j[1]>mmax*0.5:
                #         totaln+=1
                # for j in mmax*0.5:
                #     if j[1]>30:
                #         if int(j[0]) in standard:
                #             correctn+=1

                #wlj
                for j in total:
                    if int(j[0]) in standard:
                        if j[1]>10:
                            correctn+=1

                # if totaln==0:
                #     continue
                if correctn==0:
                    continue
                summary["img_id"].append(usrrecord[0])
                summary["correct"].append(correctn)
                summary["wrong"].append(wrongn)
                summary["miss"].append(missn)
                summary["totalcorrect"].append(totaln)
                summary["rate"].append(correctn/totaln)
        #print(summary)
        finalData = pd.DataFrame(summary)
        plt.rcParams['font.sans-serif'] = 'Microsoft YaHei'
        tick_label = list(finalData.iterrows())
        x = np.arange(len(tick_label))
        if len(tick_label)<4:
            continue
        y1 = finalData['correct'].values.tolist()
        y2 = finalData['totalcorrect'].values.tolist()
        y3 = finalData['rate'].values.tolist()
        fig=plt.figure(figsize=(15, 9))
        ax1=fig.add_subplot(111)
        bar_with = 0.5  # 柱体宽度plt.figure(figsize = (12,6)) #画布大小
        #print(y1,y2)
        ax1.bar(x, y1, width=bar_with,  # 柱体宽度
                align='center',  # x轴上的坐标与柱体对其的位置
                color='blue', alpha=0.6,  # 柱体透明度
                label='correct')
        # ax1.bar(x, y2, width=bar_with,
        #         # bottom=y1,  # 柱体基线的y轴坐标
        #         align='center', color='red', alpha=0.6, label='totalcorrect')
        # ax2=ax1.twinx()
        # ax2.plot(x,y3,label="rate")
        # ax2.set_ylim(0,1.2)

        plt.title('Correct Rate', fontsize=10)  # 设置x轴标题

        # plt.ylim(0, 50)

        plt.legend(loc='center', bbox_to_anchor=(0.77, 1.1), ncol=2)
        #plt.show()
        plt.draw()
        plt.savefig("./"+mode+"TimeSeries/pic-{}.png".format(img[0]))
        #plt.show()
        plt.close(fig)



