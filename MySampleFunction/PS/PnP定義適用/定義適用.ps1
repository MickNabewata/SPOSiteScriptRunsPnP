# SharePoint Online�ɐڑ�����A�J�E���g�ƃp�X���[�h����͂��܂��B
$cred = Get-Credential

# SharePoint Online�T�C�g�ɐڑ����܂��B
Connect-PnPOnline -Url https://yourdomain.sharepoint.com/sites/siteUrl -Credential $cred

# ���ݐڑ����̃T�C�g�ɒ�`�t�@�C����K�p���܂��B
Apply-PnPProvisioningTemplate -Path template.xml