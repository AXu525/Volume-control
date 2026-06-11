// 获取 DOM 元素
const volumeDisplay = document.getElementById('volumeValue');
const volumeInput = document.getElementById('volumeInput');
const volumeSlider = document.getElementById('volumeSlider');
const applyBtn = document.getElementById('applyBtn');
const statusText = document.getElementById('statusText');

// 当前音量
let currentVolume = 50;

// 初始化
document.addEventListener('DOMContentLoaded', () => {
  loadCurrentVolume();
  setupEventListeners();
});

// 设置事件监听
function setupEventListeners() {
  // 输入框回车
  volumeInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') {
      applyVolume();
    }
  });

  // 输入框失去焦点
  volumeInput.addEventListener('blur', () => {
    applyVolume();
  });

  // 应用按钮点击
  applyBtn.addEventListener('click', applyVolume);

  // 滑块值改变
  volumeSlider.addEventListener('input', (e) => {
    const volume = parseInt(e.target.value);
    updateUI(volume);
    setVolumeOnPage(volume);
  });
}

// 加载当前页面音量
async function loadCurrentVolume() {
  try {
    // 获取当前标签页
    const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });

    if (!tab) {
      showStatus('无法获取当前标签页', 'error');
      return;
    }

    // 注入内容脚本并获取当前音量
    const results = await chrome.scripting.executeScript({
      target: { tabId: tab.id },
      func: getPageVolume
    });

    if (results && results[0] && results[0].result !== undefined) {
      currentVolume = results[0].result;
      updateUI(currentVolume);
      showStatus('已加载当前页面音量', 'success');
    } else {
      showStatus('未找到可控制的音频元素', 'error');
    }
  } catch (error) {
    console.error('加载音量失败:', error);
    showStatus('加载音量失败: ' + error.message, 'error');
  }
}

// 获取页面音量的函数（注入到页面中执行）
function getPageVolume() {
  // 查找所有音频和视频元素
  const audioElements = document.querySelectorAll('audio, video');
  const mediaElements = Array.from(audioElements).filter(el => el.volume !== undefined);

  if (mediaElements.length === 0) {
    return null;
  }

  // 返回第一个媒体元素的音量（转换为 0-100）
  const volume = mediaElements[0].volume;
  return Math.round(volume * 100);
}

// 应用音量
async function applyVolume() {
  const volume = parseInt(volumeInput.value);

  if (isNaN(volume) || volume < 0 || volume > 100) {
    showStatus('请输入有效的音量值（0-100）', 'error');
    return;
  }

  await setVolumeOnPage(volume);
}

// 设置页面音量
async function setVolumeOnPage(volume) {
  try {
    // 获取当前标签页
    const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });

    if (!tab) {
      showStatus('无法获取当前标签页', 'error');
      return;
    }

    // 注入内容脚本并设置音量
    await chrome.scripting.executeScript({
      target: { tabId: tab.id },
      func: setPageVolume,
      args: [volume]
    });

    currentVolume = volume;
    showStatus('音量已设置', 'success');
  } catch (error) {
    console.error('设置音量失败:', error);
    showStatus('设置音量失败: ' + error.message, 'error');
  }
}

// 设置页面音量的函数（注入到页面中执行）
function setPageVolume(volume) {
  // 查找所有音频和视频元素
  const audioElements = document.querySelectorAll('audio, video');
  const mediaElements = Array.from(audioElements).filter(el => el.volume !== undefined);

  // 设置每个媒体元素的音量
  mediaElements.forEach(el => {
    el.volume = volume / 100;
  });
}

// 更新 UI
function updateUI(volume) {
  volumeDisplay.textContent = volume;
  volumeInput.value = volume;
  volumeSlider.value = volume;
}

// 显示状态信息
function showStatus(message, type = '') {
  statusText.textContent = message;
  statusText.className = 'status' + (type ? ' ' + type : '');

  // 3 秒后清除状态
  setTimeout(() => {
    statusText.textContent = '就绪';
    statusText.className = 'status';
  }, 3000);
}
