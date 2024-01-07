## 動作確認環境  
windows11  


## 動いているところを確認する方法  
https://drive.google.com/file/d/1Ol2kOKf9dJzViVQCDb2BMcbfXXc5P4UH/view?usp=drive_link

上記のリンクからzipをダウンロード<br>
解凍後中のUnityProject.exeを実行する<br>
（ダウンロードの際に出るエラーは無視、exe実行時の警告も無視）<br>

## タイトル
![タイトル表示](images/title.png "title")<br>


## タウンパート
![タウン開始](images/town_main.png "town")<br>
タウンでは生産施設を建設して住人の需要に応えることでお金を稼ぐことが出来る。<br>
お金を使ってさまざまなものを購入することができるようになる。<br>

タウンパートの流れ
1.住居を建ててキャラクターを住まわせる<br>
![住居](images/assign_residence.png "assign_residence")<br>

2.生産施設を建てて労働者を割り当てる<br>
![労働者](images/assign_worker.png "assign_worker")<br>

3.サプライチェーンを構築する<br>
![チェーン1](images/chain_build1.png "chain_build1")<br>
![チェーン2](images/chain_build2.png "chain_build2")<br>
![チェーン3](images/chain_build3.png "chain_build3")<br>

生産施設は流通半径内にある建物に生産物を提供することができる<br>
生産物の中には原料を必要とするものがあるので原料を生産する施設の近くに生産施設を建てる<br>

4.マーケットを介して住人に生産物を提供する<br>
![チェーン4](images/chain_build4.png "chain_build4")<br>
住人に生産物を提供するにはマーケットが必要。<br>
生産施設と住居の間にマーケットを設置することで生産物を届けることができる。<br>
住人は需要に応じて生産物を消費し、お金を生み出す。<br>


## バトルパート
タウンからクエストを開始するとバトルが開始される<br>
![バトル](images/battle.png "battle")<br>
バトルは開始すると自動で進行する。<br>
一番後ろにいるリーダーを倒すことで勝利できる。<br>
・ForeSiwthで前・中列の入れ替えができる<br>
・AftSwitchで中・後列の入れ替えができる<br>
・Rotationで前・中・後列の入れ替えができる<br>

## 部隊編成
タウンからorganize部隊編成を開始すると編成画面になる<br>
![編成](images/organize.png "organize")<br>
バトルパートで使用する部隊のユニット編成を変更できる。<br>
ユニットを選択してタウンパートの住人を割り当てることができる。<br>
住人が割り当てられているユニットはアビリティなどで強化される。<br>


## 戦略パート
タウンから戦略マップを開始すると戦略パートが開始される<br>
![戦略マップ](images/stragegy.png "stragegy")<br>
1.部隊を選択して3Dマップの青い丸をクリックすることで部隊を配置できる<br>
2.配置したユニットを選び移動先を選択すると移動する<br>
3.移動先に敵がいた場合はバトルが始まる<br>


## キャラクター追加
タウンからキャラクター追加を選ぶと開始される
![キャラクター追加](images/gacha.png "gacha")<br>
Stable Diffusion web UIをローカルPCに導入し、APIが動かせる場合はAIでキャラ生成ができる
https://github.com/AUTOMATIC1111/stable-diffusion-webui