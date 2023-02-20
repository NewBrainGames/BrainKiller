from img_io import *
import glob as glob
import os

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

def summaryrecord(records):
    indexs=[]
    for record in records:
        mmax=-1
        index=[]
        for item in record:
            if item[1]==-1:
                continue
            if item[1]>mmax:
                mmax=item[1]
        for item in record:
            if item[1]==-1:
                continue
            if item[1]>mmax*0.5:
                index.append(int(item[0]))
        indexs.append(index)
    return indexs

def modifyswc(name,record,gold,savedict):
    '''

    :param name: filename without suffix
    :param record: correct,wrong,miss record of the name file
    :param gold: gold standard of the name file
    :param savedict: the directory where to save the modified swc
    :return:

    '''
    swcname = r"E:\A_unity\production\Shooter Game 1\Assets\StreamingAssets\ImageResources\dendriteSWC" + "\\" + name + "swc_sorted.swc"
    swc=Readswc(swcname)
    bps=FindBranchingPoints(swc)
    swc[:,5]=1
    swc[:,1]=2
    #Writeswc(swc,savedict+"\\"+name+".swc")
    for i in bps:
        swc[i-1][1]=3
    for i in gold[1]:
        swc[i-1][5]=3
    indexs=summaryrecord(record)
    for i in indexs[0]:
        if i in gold[1]:
            swc[i-1][1]=4
        else:
            swc[i-1][5]=4
    for i in indexs[1]:
        if i in gold[1]:
            swc[i-1][5]=5
        else:
            swc[i-1][1]=5
    for i in indexs[2]:
        if i in gold[1]:
            swc[i-1][1]=6
        else:
            swc[i-1][5]=6
    swc[0][1]=10
    swc[0][5]=2
    Writeswc(swc,savedict+"\\"+name+".swc")

def initswc():
    swcfiles = glob.glob("D:\A_result\game\gameinitswc\*.swc")
    for swc in swcfiles:
        name=swc.split("\\")[-1]
        swcdata=Readswc(swc)
        bps=FindBranchingPoints(swcdata)
        swcdata[:, 5] = 1
        swcdata[:, 1] = 2
        for i in bps:
            swcdata[i-1][5]=3
        swcdata[0][1] = 10
        swcdata[0][5] = 2
        Writeswc(swcdata,"D:\A_result\game\gameinitswcmodified"+"\\"+name)
def batchsnap():
    v3d=r"E:\Downloads\Vaa3D_v6.007_Windows_64bit\Vaa3D_v6.007_Windows_64bit\Vaa3D-x.exe"
    plugin=r"E:\Downloads\Vaa3D_v6.007_Windows_64bit\Vaa3D_v6.007_Windows_64bit\plugins\GameSnap\GameSnap.dll"
    #swcfiles=glob.glob("D:\A_result\game\gameswc\*.swc")
    swcfiles = glob.glob("D:\A_result\game\gameinitswcmodified\*.swc")
    for swc in swcfiles:
        cmd=v3d+" /x "+plugin+" /f snap /i "+swc+" /o D:\A_result\game\gameinitswcsnap /p 0 0 0"
        print(cmd)
        os.system(cmd)

if __name__=="__main__":
    # modifyswc()
    #print("1")
    batchsnap()
    #initswc()