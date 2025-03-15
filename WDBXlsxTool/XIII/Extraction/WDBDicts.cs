﻿namespace WDBXlsxTool.XIII.Extraction
{
    internal class WDBDicts
    {
        public static readonly Dictionary<string, string> RecordIDs = new()
        {
            // db/resident
            { "auto_clip", "AutoClip" },
            { "white", "Resident" },
            { "sound_fileid_dic", "SoundFileIdDic" },
            { "sound_fileid_dic_us", "SoundFileIdDic" },
            { "sound_filename_dic", "SoundFileNameDic" },
            { "sound_filename_dic_us", "SoundFileNameDic" },
            { "treasurebox", "TreasureBox" },
            { "zonelist", "ZoneList" },
            { "monster_book", "MonsterBook" },
            { "savepoint", "savepoint" },
            { "script", "Script" },
            { "bt_chainbonus", "bt_chainbonus" },
            { "bt_chara_prop", "BattleCharaProp" },
            { "bt_constants", "BattleConstants" },
            { "item", "Item" },
            { "item_consume", "item_consume" },
            { "special_ability", "SpecialAbility" },
            { "item_weapon", "ItemWeapon" },
            { "party", "Party" },
            { "succession", "Succession" },
            { "bt_summon", "bt_summon" },
            { "movie", "movie" },
            { "actioneffect", "ActionEffect" },
            { "attreffect", "AttributeEffectResource" },
            { "attreffectstate", "AttributeEffectStateResource" },
            { "bt_ability", "BattleAbility" },
            { "mapset", "MapSet" },
            { "emotion_voice", "EmotionVoice" },
            { "eventflag", "EventFlag" },
            { "shop", "Shop" },
            { "bt_auto_ability", "BattleAutoAbility" },
            { "charaset", "CharaSet" },

            // win32
            { "movie_items.win32", "movie_items" },
            { "movie_items_us.win32", "movie_items" },

            // ps3
            { "movie_items.ps3", "movie_items_ps3" },
            { "movie_items_us.ps3", "movie_items_ps3" },

            // x360
            { "movie_items.x360", "movie_items" },
            { "movie_items_us.x360", "movie_items" },

            // zone/z###
            { "z000", "Zone" },
            { "z001", "Zone" },
            { "z002", "Zone" },
            { "z003", "Zone" },
            { "z004", "Zone" },
            { "z005", "Zone" },
            { "z006", "Zone" },
            { "z007", "Zone" },
            { "z008", "Zone" },
            { "z009", "Zone" },
            { "z010", "Zone" },
            { "z015", "Zone" },
            { "z016", "Zone" },
            { "z017", "Zone" },
            { "z018", "Zone" },
            { "z019", "Zone" },
            { "z020", "Zone" },
            { "z021", "Zone" },
            { "z022", "Zone" },
            { "z023", "Zone" },
            { "z024", "Zone" },
            { "z025", "Zone" },
            { "z026", "Zone" },
            { "z027", "Zone" },
            { "z028", "Zone" },
            { "z029", "Zone" },
            { "z030", "Zone" },
            { "z031", "Zone" },
            { "z032", "Zone" },
            { "z033", "Zone" },
            { "z034", "Zone" },
            { "z035", "Zone" },
            { "z100", "Zone" },
            { "z101", "Zone" },
            { "z102", "Zone" },
            { "z103", "Zone" },
            { "z104", "Zone" },
            { "z105", "Zone" },
            { "z106", "Zone" },
            { "z107", "Zone" },
            { "z111", "Zone" },
            { "z200", "Zone" },
            { "z201", "Zone" },
            { "z202", "Zone" },
            { "z203", "Zone" },
            { "z204", "Zone" },
            { "z205", "Zone" },
            { "z206", "Zone" },
            { "z207", "Zone" },
            { "z208", "Zone" },
            { "z209", "Zone" },
            { "z210", "Zone" },
            { "z255", "Zone" },

            // db/script
            { "script00001", "Script" },
            { "script00002", "Script" },
            { "script00003", "Script" },
            { "script00004", "Script" },
            { "script00006", "Script" },
            { "script00008", "Script" },
            { "script00010", "Script" },
            { "script00015", "Script" },
            { "script00016", "Script" },
            { "script00017", "Script" },
            { "script00018", "Script" },
            { "script00019", "Script" },
            { "script00020", "Script" },
            { "script00021", "Script" },
            { "script00022", "Script" },
            { "script00023", "Script" },
            { "script00024", "Script" },
            { "script00026", "Script" },
            { "script00027", "Script" },
            { "script00029", "Script" },
            { "script00030", "Script" },
            { "script00105", "Script" },
            { "script00106", "Script" },

            // db/crystal
            { "crystal_fang", "crystal" },
            { "crystal_hope", "crystal" },
            { "crystal_lightning", "crystal" },
            { "crystal_sazh", "crystal" },
            { "crystal_snow", "crystal" },
            { "crystal_vanille", "crystal" },

            //db/bg
            { "mapset_loc002", "MapSet" },
            { "mapset_loc005", "MapSet" },
            { "mapset_loc006", "MapSet" },
            { "mapset_loc007", "MapSet" },
            { "mapset_loc008", "MapSet" },
            { "mapset_loc010", "MapSet" },
            { "mapset_loc012", "MapSet" },
            { "mapset_loc013", "MapSet" },
            { "mapset_loc014", "MapSet" },
            { "mapset_loc015", "MapSet" },
            { "mapset_loc016", "MapSet" },
            { "mapset_loc017", "MapSet" },
            { "mapset_loc018", "MapSet" },
            { "mapset_loc019", "MapSet" },
            { "mapset_loc020", "MapSet" },
            { "mapset_loc021", "MapSet" },
            { "mapset_loc022", "MapSet" },
            { "mapset_loc023", "MapSet" },
            { "mapset_loc024", "MapSet" },
            { "mapset_loc025", "MapSet" },
            { "mapset_loc026", "MapSet" },
            { "mapset_loc027", "MapSet" },
            { "mapset_loc029", "MapSet" },
            { "mapset_loc030", "MapSet" },
            { "mapset_loc102", "MapSet" },
            { "mapset_loc103", "MapSet" },
            { "mapset_loc104", "MapSet" },
            { "mapset_loc105", "MapSet" },
            { "mapset_loc107", "MapSet" },
        };


        public static readonly Dictionary<string, List<string>> FieldNames = new()
        {
            { "AutoClip",
                new List<string>()
                {
                    "sTitle", "sTarget", "sTarget2", "sText", "sPicture", "u4Category", "u7Sort",
                    "u4Chapter"
                }
            },

            { "Resident",
                new List<string>()
                {
                    "fVal", "iVal1", "sResourceName", "fPosX", "fPosY", "fPosZ"
                }
            },

            { "SoundFileIdDic",
                new List<string>()
                {
                    "i31FileId", "u1IsStream"
                }
            },

            { "SoundFileNameDic",
                new List<string>()
                {
                    "sResourceName"
                }
            },

            { "TreasureBox",
                new List<string>()
                {
                    "sItemResourceId", "iItemCount", "sNextTreasureBoxResourceId"
                }
            },

            { "movie_items",
                new List<string>()
                {
                    "sZoneNumber", "uCinemaSize", "uReserved", "uCinemaStart"
                }
            },

            { "movie_items_ps3",
                new List<string>()
                {
                    "sZoneNumber", "uCinemaSize", "u64CinemaStart"
                }
            },

            { "ZoneList",
                new List<string>()
                {
                    "fMovieTotalTimeSec", "iImageSize", "u8RefZoneNum0", "u8RefZoneNum1", "u8RefZoneNum2",
                    "u8RefZoneNum3", "u8RefZoneNum4", "u8RefZoneNum5", "u8RefZoneNum6", "u8RefZoneNum7",
                    "u8RefZoneNum8", "u8RefZoneNum9", "u8RefZoneNum10", "u1OnDisk0", "u1OnDisk1", "u1OnDisk2",
                    "u1OnDisk3", "u1On1stLayerPS3", "u1On2ndtLayerPS3"
                }
            },

            { "MonsterBook",
                new List<string>()
                {
                    "u6MbookId", "u9SortId", "u9PictureId", "u1Unk"
                }
            },

            { "Zone",
                new List<string>()
                {
                    "iBaseNum", "sName0", "sName1"
                }
            },

            { "savepoint",
                new List<string>()
                {
                    "sLoadScriptId", "i17PartyPositionMarkerGroupIndex",
                    "u15SaveIconBackgroundImageIndex", "i16SaveIconOverrideImageIndex"
                }
            },

            { "Script",
                new List<string>()
                {
                    "sClassName", "sMethodName", "iAdditionalArgCount", "iAdditionalArg0", "iAdditionalArg1",
                    "iAdditionalArg2", "iAdditionalArg3", "iAdditionalStringArgCount", "sAdditionalStringArg0",
                    "sAdditionalStringArg1", "sAdditionalStringArg2"
                }
            },

            { "bt_chainbonus",
                new List<string>()
                {
                    "u6WhoFrom", "u6When0", "u6When1", "u6When2", "u6WhatState", "u6WhoTo", "u6DoWhat",
                    "u6Where", "u6How", "u16Bonus"
                }
            },

            { "BattleCharaProp",
                new List<string>()
                {
                    "sInfoStrId", "sOpenCondArgS0", "u1NoLibra", "u8OpenCond", "u8AiOrderEn", "u8AiOrderJm",
                    "u4FlavorAtk", "u4FlavorBla", "u4FlavorDef"
                }
            },

            { "BattleConstants",
                new List<string>()
                {
                    "iiVal", "ffVal", "ssVal"
                }
            },

            { "Item",
                new List<string>()
                {
                    "sItemNameStringId", "sHelpStringId", "sScriptId", "uPurchasePrice", "uSellPrice",
                    "u8MenuIcon", "u8ItemCategory", "i16ScriptArg0", "i16ScriptArg1", "u1IsUseBattleMenu",
                    "u1IsUseMenu", "u1IsDisposable", "u1IsSellable", "u5Rank", "u6Genre", "u1IsIgnoreGenre",
                    "u16SortAllByKCategory", "u16SortCategoryByCategory", "u16Experience", "i8Mulitplier",
                    "u1IsUseItemChange"
                }
            },

            { "item_consume",
                new List<string>()
                {
                    "sAbilityId", "sLearnAbilityId", "u1IsUseRemodel", "u1IsUseGrow", "u16ConsumeAP"
                }
            },

            { "SpecialAbility",
                new List<string>()
                {
                    "sAbility", "u6Genre", "u3Count"
                }
            },

            // partial
            { "ItemWeapon",
                new List<string>()
                {
                    "sWeaponCharaSpecId", "sWeaponCharaSpecId2", "sAbility", "sAbility2", "sAbility3",
                    "sUpgradeAbility", "sAbilityHelpStringId", "uBuyPriceIncrement", "uSellPriceIncrement",
                    "sDisasItem1", "sDisasItem2", "sDisasItem3", "sDisasItem4", "sDisasItem5", "u8UnkVal1",
                    "u8UnkVal2", "u2UnkVal3", "u7MaxLvl", "u4UnkVal4", "u1UnkBool1", "u1UnkBool2", "u1UnkBool3",
                    "i10ExpRate1", "i10ExpRate2", "i10ExpRate3", "u1UnkBool4", "u1UnkBool5", "u8StatusModKind0",
                    "u8StatusModKind1", "u4StatusModType", "u1UnkBool6", "u1UnkBool7", "u16UnkVal5",
                    "i16StatusModVal", "u16UnkVal6", "i16AttackModVal", "u16UnkVal7", "i16MagicModVal",
                    "i16AtbModVal", "u16UnkVal8", "u16UnkVal9", "u16UnkVal10", "u14DisasRate1", "u7UnkVal11",
                    "u7UnkVal12", "u14DisasRate2", "u14DisasRate3", "u7UnkVal13", "u14DisasRate4",
                    "u7UnkVal14", "u14DisasRate5"
                }
            },

            { "Party",
                new List<string>()
                {
                    "sCharaSpecId", "sSubCharaSpecId0", "sSubCharaSpecId1", "sSubCharaSpecId2",
                    "sSubCharaSpecId3", "sSubCharaSpecId4", "sSubCharaSpecId5", "sSubCharaSpecId6",
                    "sSubCharaSpecId7", "sSubCharaSpecId8", "sRideObjectCharaSpecId0",
                    "sRideObjectCharaSpecId1", "sFieldFreeCameraSettingResourceId", "sIconResourceId",
                    "sScriptIdOnPartyCharaAIStarted", "sScriptIdOnIdle", "sBattleCharaSpecId", "sSummonId",
                    "fStopDistance", "fWalkDistance", "fPlayerRestraint", "u1IsEnableUserControl",
                    "u5OrderNumForCrest", "u8OrderNumForTool", "u7Expresspower", "u7Willpower",
                    "u7Brightness", "u7Cognition"
                }
            },

            {
                "Succession",
                new List<string>()
                {
                    "u1RideOffChocobo", "i2NaviMapMode", "i2PartyCharaAIMode", "i2UserControlMode",
                    "i9ZoneStateChangeTriggerOnEnter","i9ZoneStateWait", "u1EventSkipAble",
                    "u1FieldCommonObjectHide", "u1EnablePause", "u1SuspendFieldObject",
                    "u1DisableTalk", "i9ZoneStateChangeTriggerOnExit", "i9ZoneStateExit",
                    "u13CameraInterporationTimeOnEnter", "u1FieldActiveFlag",
                    "u13CameraInterporationTimeOnExit", "u1HighModelEventFlag",
                    "u1ApplyFieldCameraByPlayerMatrix"
                }
            },

            {
                "bt_summon",
                new List<string>()
                {
                    "iSummonKind", "sCharaSet", "sBtChSpec0", "sBtChSpec1", "sSummonInEv", "sDriveInEv",
                    "sFinishArtsEv", "iMaxSp0", "iMaxSp1", "iMaxSp2", "iMaxSp3", "iMaxSp4", "iMaxSp5",
                    "iMaxSp6", "iMaxSp7", "iMaxSp8", "iMaxSp9", "iMaxSp10", "iMaxSp11", "iMaxSp12",
                    "iMaxSp13", "iMaxSp14", "iMaxSp15", "iMaxSp16", "u16Str0", "u16Str1", "u16Str2",
                    "u16Str3", "u16Str4", "u16Str5", "u16Str6", "u16Str7", "u16Str8", "u16Str9", "u16Str10",
                    "u16Str11", "u16Str12", "u16Str13", "u16Str14", "u16Str15", "u16Str16", "u16Mag0",
                    "u16Mag1", "u16Mag2", "u16Mag3", "u16Mag4", "u16Mag5", "u16Mag6", "u16Mag7", "u16Mag8",
                    "u16Mag9", "u16Mag10", "u16Mag11", "u16Mag12", "u16Mag13", "u16Mag14", "u16Mag15",
                    "u16Mag16"
                }
            },

            {
                "crystal",
                new List<string>()
                {
                    "uCPCost", "sAbilityID", "u4Role", "u4CrystalStage", "u8NodeType", "u16NodeVal"
                }
            },

            {
                "movie",
                new List<string>()
                {
                    "sZone0", "sZone1"
                }
            },

            {
                "ActionEffect",
                new List<string>()
                {
                    "sEffectId", "iEffectArg1", "sSoundId"
                }
            },

            {
                "AttributeEffectResource",
                new List<string>()
                {
                    "sFootSoundResourceNameDefaultAttr", "sFootSoundResourceNameDrySoilAttr",
                    "sFootSoundResourceNameDampSoilAttr", "sFootSoundResourceNameGrassAttr",
                    "sFootSoundResourceNameBushAttr", "sFootSoundResourceNameSandAttr",
                    "sFootSoundResourceNameWoodAttr", "sFootSoundResourceNameBoardAttr",
                    "sFootSoundResourceNameFlooringAttr", "sFootSoundResourceNameStoneAttr",
                    "sFootSoundResourceNameGravelAttr", "sFootSoundResourceNameIronAttr",
                    "sFootSoundResourceNameThinIronAttr", "sFootSoundResourceNameClothAttr",
                    "sFootSoundResourceNameEartenwareAttr", "sFootSoundResourceNameCrystalAttr",
                    "sFootSoundResourceNameGlassAttr", "sFootSoundResourceNameIceAttr",
                    "sFootSoundResourceNameWaterAttr", "sFootSoundResourceNameAsphaltAttr",
                    "sFootSoundResourceNameNoneAttr", "sFootSoundResourceNameWireNetAttr",
                    "sFootSoundResourceNameBranchOfMachineAttr", "sFootSoundResourceNameBranchOfNatureAttr",
                    "sFootSoundResourceNameCorkAttr", "sFootSoundResourceNameMarbleAttr",
                    "sFootSoundResourceNameHologramAttr", "sFootVfxResourceNameDefaultAttr",
                    "sFootVfxResourceNameDrySoilAttr", "sFootVfxResourceNameDampSoilAttr",
                    "sFootVfxResourceNameGrassAttr", "sFootVfxResourceNameBushAttr",
                    "sFootVfxResourceNameSandAttr", "sFootVfxResourceNameWoodAttr",
                    "sFootVfxResourceNameBoardAttr", "sFootVfxResourceNameFlooringAttr",
                    "sFootVfxResourceNameStoneAttr", "sFootVfxResourceNameGravelAttr",
                    "sFootVfxResourceNameIronAttr", "sFootVfxResourceNameThinIronAttr",
                    "sFootVfxResourceNameClothAttr", "sFootVfxResourceNameEartenwareAttr",
                    "sFootVfxResourceNameCrystalAttr", "sFootVfxResourceNameGlassAttr",
                    "sFootVfxResourceNameIceAttr", "sFootVfxResourceNameWaterAttr",
                    "sFootVfxResourceNameAsphaltAttr", "sFootVfxResourceNameNoneAttr",
                    "sFootVfxResourceNameWireNetAttr", "sFootVfxResourceNameBranchOfMachineAttr",
                    "sFootVfxResourceNameBranchOfNatureAttr", "sFootVfxResourceNameCorkAttr",
                    "sFootVfxResourceNameMarbleAttr", "sFootVfxResourceNameHologramAttr"
                }
            },

            {
                "AttributeEffectStateResource",
                new List<string>()
                {
                    "sWalk", "sRun", "sJump", "sRetreat", "sLanding", "sSliding", "sSquat", "sStand", "sFly"
                }
            },

            {
                "BattleAbility",
                new List<string>()
                {
                    "sStringResId", "sInfoStResId", "sScriptId", "sAblArgStr0", "sAblArgStr1",
                    "sAutoAblStEff0", "fDistanceMin", "fDistanceMax", "fMaxJumpHeight", "fYDistanceMin",
                    "fYDistanceMax", "fAirJpHeight", "fAirJpTime", "sReplaceAirAttack", "sReplaceAirAir",
                    "sReplaceRangeAtk", "sReplaceFinAtk", "sReplaceEnAttr", "iExceptionID", "sActionId0",
                    "sActionId1", "sActionId2", "sActionId3", "sRtDamSrc", "sRefDamSrc", "sSubRefDamSrc",
                    "sSlamDamSrc", "sCamArtsSeqId0", "sCamArtsSeqId1", "sCamArtsSeqId2", "sCamArtsSeqId3",
                    "sRedirectAbility0", "sRedirectTo0", "sRedirectAbility1", "sRedirectTo1",
                    "sRedirectAbility2", "sRedirectTo2", "sRedirectAbility3", "sRedirectTo3", "sSysEffId0",
                    "iSysEffArg0", "sSysSndId0", "sRtEffId0", "iRtEffArg0", "sRtSndId0", "sRtEffId1",
                    "iRtEffArg1", "sRtSndId1", "sRtEffId2", "iRtEffArg2", "sRtSndId2", "sRtEffId3",
                    "iRtEffArg3", "sRtSndId3", "sRtEffId4", "iRtEffArg4", "sRtSndId4", "u1ComAbility",
                    "u1RsvFlag0", "u1RsvFlag1", "u1RsvFlag2", "u1RsvFlag3", "u1RsvFlag4", "u1RsvFlag5",
                    "u1RsvFlag6", "u4ArtsNameHideKd", "u16ArtsNameFrame", "u4UseRole", "u8AblSndKind",
                    "u4MenuCategory", "i16MenuSortNo", "u1NoDespel", "i16ScriptArg0", "i16ScriptArg1",
                    "u8AbilityKind", "u4TargetListKind", "i16AblArgInt0", "u4UpAblKind", "i16AblArgInt1",
                    "i16AtbCount", "i16AtRnd", "i16KeepVal", "i16IntRsv0", "i16IntRsv1", "u1TgFoge",
                    "u1NoBackStep", "u1AIWanderFlag", "u16TgElemId", "u10OpProp0", "u1AutoAblStEfEd0",
                    "u1CheckAutoRpl", "u1SeqParts", "i16AutoAblStEfTi0", "u4YRgCheckType", "u4AtDistKind",
                    "u4JumpAttackType", "u1SeqTermination", "u5ActSelType", "u4LoopFinCond", "u16LoopFinArg",
                    "u4RedirectMargeNof0", "i16RefDamSrcRpt", "i16SubRefDamSrcRp", "i8AreaRad",
                    "u8CamArtsSelType", "u4RedirectMargeNof1", "u4RedirectMargeNof2", "u4RedirectMargeNof3",
                    "u16SysEffPos0", "u16RtEffPos0", "u16RtEffPos1", "u16RtEffPos2", "u16RtEffPos3",
                    "u16RtEffPos4"
                }
            },

            {
                "MapSet",
                new List<string>()
                {
                    "iMemorySizeLimit", "iVideoMemoryLimit", "sScriptIdOnLoaded", "sMapNameResourceId",
                    "sBattleFreeSpaceResourceId", "i20LoadingTime", "i11LocationNum", "i16FieldSceneDataNum",
                    "i16BattleSceneDataNum", "i12PartyPositionMarkerGroup", "i10FieldMapNum0", "i10FieldMapNum1",
                    "i10FieldMapNum2", "i10FieldMapNum3", "i10FieldMapNum4", "i10FieldMapNum5", "i10FieldMapNum6",
                    "i10FieldMapNum7", "i10FieldMapNum8", "i10FieldMapNum9", "i10FieldMapNum10",
                    "i10FieldMapNum11", "i10FieldMapNum12", "i10FieldMapNum13", "i10FieldMapNum14",
                    "i10FieldMapNum15", "i10FieldMapNum16", "i10FieldMapNum17", "i10FieldMapNum18",
                    "i10FieldMapNum19", "i10VfxMapNum0", "i10VfxMapNum1", "i10VfxMapNum2", "i10VfxMapNum3",
                    "i10BattleMapNum0", "i10BattleMapNum1", "i10BattleMapNum2", "i10BattleMapNum3",
                    "i10BattleMapNum4", "i10BattleMapNum5"
                }
            },

            {
                "EmotionVoice",
                new List<string>()
                {
                    "u4RandomMax0", "u4RandomMax1", "u4RandomMax2", "u4RandomMax3", "u4RandomMax4",
                    "u4RandomMax5", "u4RandomMax6", "u4RandomMax7", "u4RandomMax8", "u4RandomMax9",
                    "u4AIRandomMax0", "u4AIRandomMax1", "u4AIRandomMax2", "u4AIRandomMax3", "u4AIRandomMax4",
                    "u4AIRandomMax5", "u4AIRandomMax6", "u4AIRandomMax7", "u4AIRandomMax8", "u4AIRandomMax9"
                }
            },

            {
                "EventFlag",
                new List<string>()
                {
                    "iFlagIndex"
                }
            },

            {
                "Shop",
                new List<string>()
                {
                    "sFlagItemId", "sUnlockEventID", "sShopNameLabel", "sSignId", "sExplanationLabel",
                    "sUnkStringVal1", "sItemLabel1", "sItemLabel2", "sItemLabel3", "sItemLabel4",
                    "sItemLabel5", "sItemLabel6", "sItemLabel7", "sItemLabel8", "sItemLabel9", "sItemLabel10",
                    "sItemLabel11", "sItemLabel12", "sItemLabel13", "sItemLabel14", "sItemLabel15",
                    "sItemLabel16", "sItemLabel17", "sItemLabel18", "sItemLabel19", "sItemLabel20",
                    "sItemLabel21", "sItemLabel22", "sItemLabel23", "sItemLabel24", "sItemLabel25",
                    "sItemLabel26", "sItemLabel27", "sItemLabel28", "sItemLabel29", "sItemLabel30",
                    "sItemLabel31", "sItemLabel32", "u4Version", "u13ZoneNum"
                }
            },

            {
                "BattleAutoAbility",
                new List<string>()
                {
                    "sStringResId", "sInfoStResId", "sScriptId", "sAutoAblArgStr0", "sAutoAblArgStr1",
                    "u1RsvFlag0", "u1RsvFlag1", "u1RsvFlag2", "u1RsvFlag3", "u4UseRole", "u4MenuCategory",
                    "i16MenuSortNo", "i16ScriptArg0", "i16ScriptArg1", "u8AutoAblKind", "i16AutoAblArgInt0",
                    "i16AutoAblArgInt1", "i16WepLvArg0", "i16WepLvArg1"
                }
            },

            {
                "CharaSet",
                new List<string>()
                {
                    "iMemorySizeLimit", "iVideoMemorySizeLimit", "sCharaSpecId0", "sCharaSpecId1",
                    "sCharaSpecId2", "sCharaSpecId3", "sCharaSpecId4", "sCharaSpecId5", "sCharaSpecId6",
                    "sCharaSpecId7", "sCharaSpecId8", "sCharaSpecId9", "sCharaSpecId10", "sCharaSpecId11",
                    "sCharaSpecId12", "sCharaSpecId13", "sCharaSpecId14", "sCharaSpecId15", "sCharaSpecId16",
                    "sCharaSpecId17", "sCharaSpecId18", "sCharaSpecId19", "sCharaSpecId20", "sCharaSpecId21",
                    "sCharaSpecId22", "sCharaSpecId23", "sCharaSpecId24", "sCharaSpecId25", "sCharaSpecId26",
                    "sCharaSpecId27", "sCharaSpecId28", "sCharaSpecId29", "sCharaSpecId30", "sCharaSpecId31",
                    "sCharaSpecId32", "sCharaSpecId33", "sCharaSpecId34", "sCharaSpecId35", "sCharaSpecId36",
                    "sCharaSpecId37", "sCharaSpecId38", "sCharaSpecId39", "sCharaSpecId40", "sCharaSpecId41",
                    "sCharaSpecId42", "sCharaSpecId43", "sCharaSpecId44", "sCharaSpecId45", "sCharaSpecId46",
                    "sCharaSpecId47", "sCharaSpecId48", "sCharaSpecId49", "sCharaSpecId50", "sCharaSpecId51",
                    "sCharaSpecId52", "sCharaSpecId53", "sCharaSpecId54", "sCharaSpecId55", "sCharaSpecId56",
                    "sCharaSpecId57", "sCharaSpecId58", "sCharaSpecId59","sCharaSpecId60", "sCharaSpecId61",
                    "sCharaSpecId62","sCharaSpecId63", "u1PartyLoadRequestIndex0", "u1PartyLoadRequestIndex1",
                    "u1PartyLoadRequestIndex2", "u1PartyLoadRequestIndex3", "u1PartyLoadRequestIndex4",
                    "u1PartyLoadRequestIndex5"
                }
            }
        };
    }
}