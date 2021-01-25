function chorzr()
end
function setvalue(address,flags,value) chorzr('Modify address value(Address, value type, value to be modified)') local tt={} tt[1]={} tt[1].address=address tt[1].flags=flags tt[1].value=value gg.setValues(tt) end
function L()
so=gg.getRangesList('libUE4.so')[1].start
py=0x130CBF0
setvalue(so+py,16,0)
gg.toast("Less Recoil")
end
function C()
so=gg.getRangesList('libUE4.so')[1].start
py=0x130D1A8
setvalue(so+py,16,0)
gg.toast("Small Croshair")
end
function A()
so=gg.getRangesList('libUE4.so')[1].start
py=0xFB4694
setvalue(so+py,16,0)
gg.toast("AimBot")
end
function W()
so=gg.getRangesList('libUE4.so')[1].start
py=0x24A74B0
setvalue(so+py,16,0)
so=gg.getRangesList('libUE4.so')[1].start
py=0x24A74BC
setvalue(so+py,16,0)
gg.toast("AimLock")
end
function BCLR()
so=gg.getRangesList('libUE4.so')[1].start
py=0x3B65608
setvalue(so+py,16,40)
gg.toast("Magic Bullet")
end

function BS()
so=gg.getRangesList('libUE4.so')[1].start
py=0x2475D58
setvalue(so+py,16,0)
gg.toast("No Grass")
end
function BM()
so=gg.getRangesList('libUE4.so')[1].start
py=0x2C344C8
setvalue(so+py,16,0)
gg.toast("No Fog")
end
while true do
  if gg.isVisible(true) then
    XGCK = 1
    gg.setVisible(false)
  end
  gg.clearResults()
  if XGCK == 1 then
    Main()
  end
end

