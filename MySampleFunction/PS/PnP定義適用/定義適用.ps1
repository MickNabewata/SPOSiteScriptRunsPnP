# SharePoint Onlineに接続するアカウントとパスワードを入力します。
$cred = Get-Credential

# SharePoint Onlineサイトに接続します。
Connect-PnPOnline -Url https://yourdomain.sharepoint.com/sites/siteUrl -Credential $cred

# 現在接続中のサイトに定義ファイルを適用します。
Apply-PnPProvisioningTemplate -Path template.xml