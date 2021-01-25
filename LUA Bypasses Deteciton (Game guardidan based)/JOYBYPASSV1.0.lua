
--- Excute On Tencent Logo ---
local gg = gg
gg.setVisible(false)

local memFrom, memTo, lib, num, lim, results, src, ok = 0, -1, nil, 0, 32, {}, nil, false
function name(n)
if lib ~= n then
lib = n
local ranges = gg.getRangesList(lib)
if #ranges == 0 then
gg.toast("OPEN PUBGM FIRST TO EXCUTE THIS SCRIPT")
gg.setVisible(true)
os.exit()
else
memFrom = ranges[1].start
memTo = ranges[#ranges]["end"]
 end
end
end
function hex2tbl(hex)
local ret = {}
hex:gsub("%S%S", function(ch)
ret[#ret + 1] = ch
return ""
end)
return ret
end
function original(orig)
local tbl = hex2tbl(orig)
local len = #tbl
if len == 0 then
return
end
local used = len
if len > lim then
used = lim
end
local s = ""
for i = 1, used do
if i ~= 1 then
s = s .. ";"
end
local v = tbl[i]
if v == "??" or v == "**" then
v = "0~~0"
end
s = s .. v .. "r"
end
s = s .. "::" .. used
gg.searchNumber(s, gg.TYPE_BYTE, false, gg.SIGN_EQUAL, memFrom, memTo)
if len > used then
for i = used + 1, len do
local v = tbl[i]
if v == "??" or v == "**" then
v = 256
else
v = ("0x" .. v) + 0
if v > 127 then
v = v - 256
end
end
tbl[i] = v
end
end
local found = gg.getResultCount()
results = {}
local count = 0
local checked = 0
while not (found <= checked) do
local all = gg.getResults(8)
local total = #all
local start = checked
if total < checked + used then
break
end
for i, v in ipairs(all) do
v.address = v.address + offset
end
gg.loadResults(all)
while total > start do
local good = true
local offset = all[1 + start].address - 1
if len > used then
local get = {}
for i = lim + 1, len do
get[i - lim] = {
address = offset + i,
flags = gg.TYPE_BYTE,
value = 0
}
end
get = gg.getValues(get)
for i = lim + 1, len do
local ch = tbl[i]
if ch ~= 256 and get[i - lim].value ~= ch then
good = false
break
end
end
end
if good then
count = count + 1
results[count] = offset
checked = checked + used
else
local del = {}
for i = 1, used do
del[i] = all[i + start]
end
gg.removeResults(del)
end
start = start + used
end
end
end
function replaced(repl)
num = num + 1
local tbl = hex2tbl(repl)
if src ~= nil then
local source = hex2tbl(src)
for i, v in ipairs(tbl) do
if v ~= "??" and v ~= "**" and v == source[i] then
tbl[i] = "**"
end
end
src = nil
end
local cnt = #tbl
local set = {}
local s = 0
for _, addr in ipairs(results) do
for i, v in ipairs(tbl) do
if v ~= "??" and v ~= "**" then
s = s + 1
set[s] = {
address = addr + i,
value = v .. "r",
flags = gg.TYPE_BYTE
}
 end
end
end
if s ~= 0 then
gg.setValues(set)
end
ok = true
end

function ADDRX()
end
function setvalue(address,flags,value) ADDRX('Modify address value(Address, value type, value to be modified)') local tt={} tt[1]={} tt[1].address=address tt[1].flags=flags tt[1].value=value gg.setValues(tt) end

--- Crash/Ptrace ---
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x80940
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x80ED0
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()

gg.processPause()
--xxxxxxxxxxxxxxxxxxxx--------
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x130B0
original("7F 45 4C 46 01 01 01 00")
replaced("A8 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x130B4
original("7F 45 4C 46 01 01 01 00")
replaced("A8 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x130BC
original("7F 45 4C 46 01 01 01 00")
replaced("A8 00 00 00")
gg.clearResults()
--xxxxxxxxxxxxxxxxxxxx--------
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1304C
original("7F 45 4C 46 01 01 01 00")
replaced("A8 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1427E
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x14288
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15EF2
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15FB0
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1697E
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x34598
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x345FA
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00 00 00 00 00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x5B090
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 00 00 00 00 00 00")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x66044
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x6606A
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x66076
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x662B2
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x662C6
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF")
gg.clearResults()

--test disabled some pipe & heartbeat
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0xE000
original("7F 45 4C 46 01 01 01 00")
replaced("00 00 A0 E3 1E FF 2F E1")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15F34
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15F8C
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x166E4
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
----&&&xxx===
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x2FB42
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x32E44
original("7F 45 4C 46 01 01 01 00")
replaced("00 20 70 47")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x32E6E
original("7F 45 4C 46 01 01 01 00")
replaced("00 20 70 47")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x449C0
original("7F 45 4C 46 01 01 01 00")
replaced("00 20 70 47")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x5580A
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF 00 BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x5582C
original("7F 45 4C 46 01 01 01 00")
replaced("00 BF")
gg.clearResults()
-- gg.setRanges(gg.REGION_CODE_APP)
-- name("libtersafe.so")
-- offset = 0x55C16
-- original("7F 45 4C 46 01 01 01 00")
-- replaced("00 BF 00 BF 00 BF 00 BF")
-- gg.clearResults()

--ChkInit{
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15084
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x150A8
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x150D0
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x150F4
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()

--SetUserInfoEx{
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x152E8
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1530C
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15334
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15432
original("7F 45 4C 46 01 01 01 00")
replaced("72")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15436
original("7F 45 4C 46 01 01 01 00")
replaced("30")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1544C
original("7F 45 4C 46 01 01 01 00")
replaced("16")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1544E
original("7F 45 4C 46 01 01 01 00")
replaced("57")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15382
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x153A6
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x153D2
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15414
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1543C
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1545C
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15492
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x154DC
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15560
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()

--ChkSetGameStatus{
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15622
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1565E
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15682
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x156AA
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x156CE
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()

--gamechannel
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x159DC
original("7F 45 4C 46 01 01 01 00")
replaced("31")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1576E
original("7F 45 4C 46 01 01 01 00")
replaced("6A")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x1577A
original("7F 45 4C 46 01 01 01 00")
replaced("30")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x324E0
original("7F 45 4C 46 01 01 01 00")
replaced("7E")
gg.clearResults()

--GetSDKInterfaceChkState
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x4F2BC
original("7F 45 4C 46 01 01 01 00")
replaced("12")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x4F2E2
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()

--tss_get_report_data2:%d, %p
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x13C8E
original("7F 45 4C 46 01 01 01 00")
replaced("19")
gg.clearResults()

--spgp_
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15C20
original("7F 45 4C 46 01 01 01 00")
replaced("00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15C3C
original("7F 45 4C 46 01 01 01 00")
replaced("00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15C58
original("7F 45 4C 46 01 01 01 00")
replaced("00")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x14E8E
original("7F 45 4C 46 01 01 01 00")
replaced("1E")
gg.clearResults()

--get_%d:%p
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x63112
original("7F 45 4C 46 01 01 01 00")
replaced("BE")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x63134
original("7F 45 4C 46 01 01 01 00")
replaced("86")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x63166
original("7F 45 4C 46 01 01 01 00")
replaced("BE")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x63174
original("7F 45 4C 46 01 01 01 00")
replaced("47")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x631CA
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x15C04
original("7F 45 4C 46 01 01 01 00")
replaced("40")
gg.clearResults()

--|tpchannel ver:%s
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x64FF0
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x13EB8
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x63846
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()

--TssSDK::ReportQueue
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x2643A
original("7F 45 4C 46 01 01 01 00")
replaced("7E")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0xA2B58
original("7F 45 4C 46 01 01 01 00")
replaced("1E")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8DF5A
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8DF8E
original("7F 45 4C 46 01 01 01 00")
replaced("41")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8DFBA
original("7F 45 4C 46 01 01 01 00")
replaced("9B")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8DFE6
original("7F 45 4C 46 01 01 01 00")
replaced("10")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8DFF2
original("7F 45 4C 46 01 01 01 00")
replaced("58")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E048
original("7F 45 4C 46 01 01 01 00")
replaced("30")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E054
original("7F 45 4C 46 01 01 01 00")
replaced("43")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E086
original("7F 45 4C 46 01 01 01 00")
replaced("91")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E0B6
original("7F 45 4C 46 01 01 01 00")
replaced("3E")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E0CE
original("7F 45 4C 46 01 01 01 00")
replaced("F4")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E0DA
original("7F 45 4C 46 01 01 01 00")
replaced("34")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E110
original("7F 45 4C 46 01 01 01 00")
replaced("3E")
gg.clearResults()

--cs_speed_ctl
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x26608
original("7F 45 4C 46 01 01 01 00")
replaced("02")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x26870
original("7F 45 4C 46 01 01 01 00")
replaced("19")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x43052
original("7F 45 4C 46 01 01 01 00")
replaced("BF")
gg.clearResults()

--TssSDK::AntiEmulator
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x2D9F8
original("7F 45 4C 46 01 01 01 00")
replaced("42")
gg.clearResults()

gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x5CA84
original("7F 45 4C 46 01 01 01 00")
replaced("23")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x5CB82
original("7F 45 4C 46 01 01 01 00")
replaced("B3")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x9FF60
original("7F 45 4C 46 01 01 01 00")
replaced("B0")
gg.clearResults()

--TssSDK::InfoCollector
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x33826
original("7F 45 4C 46 01 01 01 00")
replaced("83")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x33886
original("7F 45 4C 46 01 01 01 00")
replaced("83")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x9EE26
original("7F 45 4C 46 01 01 01 00")
replaced("E4")
gg.clearResults()


gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x33BE4
original("7F 45 4C 46 01 01 01 00")
replaced("88")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x4F3BE
original("7F 45 4C 46 01 01 01 00")
replaced("32")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x423A2
original("7F 45 4C 46 01 01 01 00")
replaced("BF")
gg.clearResults()
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x6496E
original("7F 45 4C 46 01 01 01 00")
replaced("23")
gg.clearResults()

--TssSDK::TssInfoPublisher
gg.setRanges(gg.REGION_CODE_APP)
name("libtersafe.so")
offset = 0x8E69E
original("7F 45 4C 46 01 01 01 00")
replaced("2E")
gg.clearResults()

--lesrec
gg.getRangesList('libUE4.so')
gg.setRanges(gg.REGION_CODE_APP)
gg.searchNumber("h 7A 08 00 EB 04 00 A0 E1 05 10 A0 E1 99 09 00 EB", gg.TYPE_BYTE, false, gg.SIGN_EQUAL, 0, -1)
gg.getResults(gg.getResultsCount())
gg.editAll("h 00 00 00 00", gg.TYPE_BYTE)
gg.clearResults()

gg.processResume()

gg.toast(" Checks Patched ")
gg.alert(" SinkiCheat.com ")