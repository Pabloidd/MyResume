<script>
  import toast from 'svelte-french-toast';
  
  export let id;
  export let title;
  export let initialStatus = 'public'; // 'public' или 'private'
  
  const RESUME_API_BASE = 'http://localhost:5052';
  let status = initialStatus;
  let isUpdating = false;
  
  function getStatusText() {
    return status === 'public' ? 'ПУБЛИЧНО' : 'ПРИВАТНО';
  }
  
  async function toggleStatus() {
    if (isUpdating) return;
    
    isUpdating = true;
    try {
      const response = await fetch(`${RESUME_API_BASE}/api/resumes/${id}/toggle-status`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' }
      });
      
      if (!response.ok) throw new Error('Ошибка при смене статуса');
      
      const data = await response.json();
      status = data.status; // Предполагаем, что бэкенд возвращает новый статус
      toast.success(`Статус изменен на ${getStatusText()}`);
    } catch (error) {
      toast.error(error.message);
    } finally {
      isUpdating = false;
    }
  }
  
  async function handleDownload() {
    try {
      const response = await fetch(`${RESUME_API_BASE}/api/resumes/${id}/download`);
      if (!response.ok) throw new Error('Ошибка при скачивании');
      
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${title}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (error) {
      toast.error(error.message);
    }
  }
</script>

<div class="resume-row">
  <span class="resume-title">{title}</span>
  
  <button 
    class="resume-status {status === 'public' ? 'status-public' : 'status-private'}" 
    class:updating={isUpdating}
    on:click={toggleStatus}
    disabled={isUpdating}
  >
    {#if isUpdating}
      ⏳
    {:else}
      {getStatusText()}
    {/if}
  </button>
  
  <button class="resume-actions" on:click={handleDownload}>
    <span class="download-icon">⬇️</span>
    <span class="download-text">Скачать</span>
  </button>
</div>

<style>
  .resume-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    padding: 1rem 0.5rem;
    border-bottom: 1px solid #e2e8f0;
    align-items: center;
    
  }

  .resume-title {
    font-weight: 500;
    color: #0f172a;
    font-size: 1.35rem;
  }

  .resume-status {
    border: none;
    font-weight: 600;
    font-size: 1.1rem;
    cursor: pointer;
    padding: 0.5rem 1rem;
    border-radius: 20px;
    text-align: left;
    width: fit-content;
    transition: all 0.2s;
  }

  /* Явно задаем цвета для каждого статуса */
  .resume-status.status-public {
    color: #059669 !important;
    background: rgba(5, 150, 105, 0.1) !important;
  }

  .resume-status.status-private {
    color: #b45309 !important;
    background: rgba(180, 83, 9, 0.1) !important;
  }

  .resume-status.updating {
    opacity: 0.7;
    cursor: wait;
  }

  .resume-actions {
    background: #2563eb;
    color: white;
    border: none;
    border-radius: 8px;
    padding: 0.5rem 1rem;
    font-size: 1rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    width: fit-content;
    transition: all 0.2s;
  }

  .resume-actions:hover {
    background: #1d4ed8;
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(37, 99, 235, 0.3);
  }

  .download-icon {
    font-size: 1.1rem;
  }

  .download-text {
    font-size: 0.95rem;
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  /* 1300px */
  @media (max-width: 1300px) {
    .resume-row {
      font-size: 1.5rem;
      padding: 0.9rem 0.4rem;
    }
    
    .resume-status {
      font-size: 1.25rem;
      padding: 0.4rem 0.9rem;
    }
    
    .resume-actions {
      padding: 0.4rem 0.9rem;
      font-size: 1.25rem;
    }
  }

  /* 1080px */
  @media (max-width: 1080px) {
    .resume-row {
      font-size: 1rem;
      padding: 0.8rem 0.3rem;
    }
    
    .resume-status {
      font-size: 0.95rem;
      padding: 0.35rem 0.8rem;
    }
    
    .resume-actions {
      padding: 0.35rem 0.8rem;
      font-size: 0.9rem;
    }
    
    .download-icon {
      font-size: 1rem;
    }
    
    .download-text {
      font-size: 0.85rem;
    }
  }

  /* 900px */
  @media (max-width: 900px) {
    .resume-row {
      grid-template-columns: 1.5fr 1fr 1fr;
      font-size: 0.95rem;
      padding: 0.7rem 0.3rem;
    }
    
    .resume-status {
      font-size: 0.85rem;
      padding: 0.3rem 0.7rem;
      border-radius: 16px;
    }
    
    .resume-actions {
      padding: 0.3rem 0.7rem;
      font-size: 0.85rem;
    }
  }

  /* 650px */
  @media (max-width: 650px) {
    .resume-row {
      grid-template-columns: 1fr;
      gap: 0.75rem;
      padding: 1rem 0.5rem;
      text-align: left;
    }
    
    .resume-title {
      font-size: 1.1rem;
      font-weight: 600;
      margin-bottom: 0.25rem;
    }
    
    .resume-status {
      width: 100%;
      text-align: center;
      justify-content: center;
      font-size: 0.9rem;
      padding: 0.5rem;
    }
    
    .resume-actions {
      width: 100%;
      justify-content: center;
      font-size: 0.9rem;
      padding: 0.5rem;
    }
  }

  /* 475px */
  @media (max-width: 475px) {
    .resume-row {
      padding: 0.8rem 0.4rem;
      gap: 0.5rem;
    }
    
    .resume-title {
      font-size: 1rem;
    }
    
    .resume-status {
      font-size: 0.85rem;
      padding: 0.4rem;
    }
    
    .resume-actions {
      font-size: 0.85rem;
      padding: 0.4rem;
    }
    
    .download-icon {
      font-size: 0.9rem;
    }
    
    .download-text {
      font-size: 0.8rem;
    }
  }
</style>