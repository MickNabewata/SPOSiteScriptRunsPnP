�E�T�C�g�X�N���v�g�o�^

	- SPO�Ǘ��V�F�����N��

		cd (script.txt�z�u�t�H���_)

		$script = Get-Content -LiteralPath .\script.txt -Raw

		$userName = "account@yourdomain.onmicrosoft.com"
		$cred = Get-Credential -UserName $userName -Message "�p�X���[�h����͂��Ă�������"
		Connect-SPOService -Url https://yourdmain-admin.sharepoint.com -Credential $cred

		Add-SPOSiteScript -Title "�T�C�g�X�N���v�g��PnP�v���r�W���j���O����" -Content $script -Description "���ؗp"

�E�T�C�g�f�U�C���o�^

	Add-SPOSiteDesign -Title "�T�C�g�X�N���v�g��PnP�v���r�W���j���O����" -WebTemplate "68" -SiteScripts "your site script id" -Description "���ؗp"

		�� 64 : �`�[���T�C�g  68 : �R�~���j�e�B�T�C�g

		�� �v���r���[�摜�� -PreviewImageUrl �Ŏw��