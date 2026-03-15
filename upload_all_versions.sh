#!/bin/bash

# 版本列表（按顺序，不包括MyLab_10WebGL）
versions=("MyLab_2" "MyLab_3" "MyLab_4" "MyLab_5" "MyLab_6" "MyLab_6failed" "MyLab_7" "MyLab_8" "MyLab_9" "MyLab_9M" "MyLab_10")

# 源文件夹路径
source_path="/Users/tuxol/Documents/DataVault/CST/#Unity3D/ReachThePeak"

# 目标文件夹路径
target_path="/Users/tuxol/Documents/DataVault/CST/#Unity3D/github/Reach-the-Peak-Unity3D"

cd "$target_path"

echo "开始上传所有版本到GitHub仓库..."

for i in "${!versions[@]}"; do
    version="${versions[$i]}"
    echo "正在处理 $version ($(($i + 1))/${#versions[@]})..."
    
    # 清空当前内容（保留.git）
    find . -maxdepth 1 -not -name '.git' -not -name '.' -not -name '..' -not -name 'upload_all_versions.sh' -exec rm -rf {} \;
    
    # 复制新版本内容
    if [ -d "$source_path/$version" ]; then
        cp -r "$source_path/$version"/* .
        
        # 添加到Git
        git add .
        
        # 提交
        git commit -m "Add $version version"
        
        # 创建分支（除了MyLab_10，它将保留在main分支）
        if [ "$version" != "MyLab_10" ]; then
            git branch "$version"
            echo "创建分支: $version"
        else
            echo "MyLab_10 保留在 main 分支"
        fi
        
        echo "$version 处理完成"
    else
        echo "错误: 找不到源文件夹 $source_path/$version"
        exit 1
    fi
    echo "---"
done

echo "所有版本上传完成！"
echo "创建的分支："
git branch
