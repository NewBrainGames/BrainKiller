import matplotlib
import numpy as np
import pandas
#import seaborn as sns
import pandas as pd
import matplotlib.pyplot as plt

matplotlib.use('TkAgg')

# 加载数据
penguins = pd.read_csv(r'D:/A_pythonwork/figure/correctbp.csv')
penguins.head()

result = {
    "img_id": [],
    "Original_Correct_Rate": [],
    "Player_Correct_Rate": []
}

for index, row in penguins.iterrows():
    print(row.values)
    imgName = row.name
    GS_BPs = row[0]
    Initial_BPs = row[1]
    Player_Submit_BPs = row[3]
    CorrectBPs__Submitted = row[2]
    # print(Player_Submit_BPs)



    # OCR = row["Original_Correct_Rate"]
    # PCR = row["Player_Correct_Rate"]
    if Player_Submit_BPs != 0:
        OCR = GS_BPs / Initial_BPs
        PCR = CorrectBPs__Submitted / Player_Submit_BPs
        result["img_id"].append(imgName)
        result["Original_Correct_Rate"].append(OCR)
        result["Player_Correct_Rate"].append(PCR)


finalData = pd.DataFrame(result)
finalData = finalData.sort_values(by=['Player_Correct_Rate'], ascending=[True])#按玩家正确率递增排序
plt.rcParams['font.sans-serif'] = 'Microsoft YaHei'

tick_label = list(finalData.iterrows())
x = np.arange(len(tick_label))

y1 = finalData['Original_Correct_Rate'].values.tolist() # 收入(剔除自己转入)
y2 = finalData['Player_Correct_Rate'].values.tolist()  # 支出(剔除自己转入)
print(y1)

bar_with = 0.5  # 柱体宽度plt.figure(figsize = (12,6)) #画布大小
plt.bar(x, y1, width=bar_with,  # 柱体宽度
        align= 'center', #x轴上的坐标与柱体对其的位置
        color = 'blue', alpha = 0.6, #柱体透明度
       label = 'Original_Correct_Rate')
plt.bar(x, y2, width=bar_with,
        # bottom=y1,  # 柱体基线的y轴坐标
        align= 'center', color = 'red', alpha = 0.6, label = 'Player_Correct_Rate')

plt.title('Correct Rate', fontsize = 10) #设置x轴标题
# plt.xticks(x + bar_with / 2, tick_label, rotation=70)  # 设置x轴坐标
# plt.xlabel('时间',fontsize = 8, verticalalignment = 'top', horizontalalignment ='right',rotation='horizontal')
# plt.xlabel('时间',fontsize = 8, verticalalignment = 'bottom', horizontalalignment ='center')

plt.ylim(0.4, 1)
# 图例设在图形外面，控制坐标参数
plt.legend(loc= 'center', bbox_to_anchor = (0.77, 1.1), ncol=2)
plt.show()
