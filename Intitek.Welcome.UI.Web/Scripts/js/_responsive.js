﻿const signUp1Button=document.getElementById('signUp-1');const signIn1Button=document.getElementById('signIn-1');if(signUp1Button){signUp1Button.addEventListener('click',()=>{container.classList.add("right-panel-active")})}
if(signIn1Button){signIn1Button.addEventListener('click',()=>{container.classList.remove("right-panel-active")})}
$(document).ready(function(){$('.mobile-menu-icon').click(function(){$('.tm-nav').toggleClass('hide animated fadeIn slow')});$('body').bind('touchstart',function(){});$('.main-wrapper').bind('click',function(){$('.tm-nav').addClass('hide animated fadeIn slow')});$('.main-wrapper').bind('touchstart',function(){$('.tm-nav').addClass('hide animated fadeIn slow')})});if(window.matchMedia('(max-width: 480px)').matches){$("#divAdmin button").attr('disabled',!0);$("#divAdmin button > i").addClass('disabled');$("#divAdmin a").attr('disabled',!0);$("#divAdmin a").addClass('disabled')}