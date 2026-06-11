// 内容脚本 - 注入到网页中
// 用于控制网页中的音频和视频元素音量

(function() {
  'use strict';

  // 检查是否已经注入
  if (window.audioVolumeControlInjected) {
    return;
  }
  window.audioVolumeControlInjected = true;

  // 默认音量
  const DEFAULT_VOLUME = 1.0;

  // 获取所有媒体元素
  function getMediaElements() {
    const audioElements = document.querySelectorAll('audio, video');
    return Array.from(audioElements).filter(el => el.volume !== undefined);
  }

  // 设置音量
  function setVolume(volume) {
    const mediaElements = getMediaElements();
    mediaElements.forEach(el => {
      el.volume = volume;
    });
    return mediaElements.length;
  }

  // 获取音量
  function getVolume() {
    const mediaElements = getMediaElements();
    if (mediaElements.length === 0) {
      return null;
    }
    return mediaElements[0].volume;
  }

  // 监听新添加的媒体元素
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      mutation.addedNodes.forEach((node) => {
        if (node.nodeType === Node.ELEMENT_NODE) {
          // 检查添加的节点是否是媒体元素
          if (node.tagName === 'AUDIO' || node.tagName === 'VIDEO') {
            node.volume = DEFAULT_VOLUME;
          }
          // 检查添加的节点是否包含媒体元素
          const mediaElements = node.querySelectorAll?.('audio, video');
          if (mediaElements) {
            mediaElements.forEach(el => {
              el.volume = DEFAULT_VOLUME;
            });
          }
        }
      });
    });
  });

  // 开始观察文档变化
  observer.observe(document.body, {
    childList: true,
    subtree: true
  });

  // 监听来自扩展的消息
  chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === 'getVolume') {
      const volume = getVolume();
      sendResponse({ volume: volume });
    } else if (request.action === 'setVolume') {
      const count = setVolume(request.volume);
      sendResponse({ count: count });
    }
    return true;
  });

  // 初始化所有现有媒体元素
  function initializeMediaElements() {
    const mediaElements = getMediaElements();
    mediaElements.forEach(el => {
      // 如果音量是 0（可能是某些网站的默认值），设置为默认音量
      if (el.volume === 0) {
        el.volume = DEFAULT_VOLUME;
      }
    });
  }

  // 页面加载完成后初始化
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeMediaElements);
  } else {
    initializeMediaElements();
  }

  console.log('[音量控制] 内容脚本已注入');
})();
