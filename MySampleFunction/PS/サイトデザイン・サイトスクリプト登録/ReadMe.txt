・サイトスクリプト登録

	- SPO管理シェルを起動

		cd (script.txt配置フォルダ)

		$script = Get-Content -LiteralPath .\script.txt -Raw

		$userName = "account@yourdomain.onmicrosoft.com"
		$cred = Get-Credential -UserName $userName -Message "パスワードを入力してください"
		Connect-SPOService -Url https://yourdmain-admin.sharepoint.com -Credential $cred

		Add-SPOSiteScript -Title "サイトスクリプト＆PnPプロビジョニング検証" -Content $script -Description "検証用"

・サイトデザイン登録

	Add-SPOSiteDesign -Title "サイトスクリプト＆PnPプロビジョニング検証" -WebTemplate "68" -SiteScripts "your site script id" -Description "検証用"

		※ 64 : チームサイト  68 : コミュニティサイト

		※ プレビュー画像は -PreviewImageUrl で指定