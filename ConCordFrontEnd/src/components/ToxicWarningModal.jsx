import { useEffect, useRef } from 'react'

export default function ToxicWarningModal({ isOpen, onClose, details }) {
  const dismissBtnRef = useRef(null)
  const onCloseRef = useRef(onClose)
  onCloseRef.current = onClose

  useEffect(() => {
    if (!isOpen) return

    dismissBtnRef.current?.focus()

    const handleKeyDown = (e) => {
      if (e.key === 'Escape') {
        onCloseRef.current?.()
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => {
      window.removeEventListener('keydown', handleKeyDown)
      document.getElementById('text')?.focus()
    }
  }, [isOpen])

  if (!isOpen) return null

  const title = details?.title || 'Toxic Language Detected'
  const message =
    details?.message ||
    'Your message was flagged as containing toxic or inappropriate language. ConCord promotes a friendly and respectful chat community. Please rephrase your message and refrain from posting toxic language.'
  const confidencePercent =
    typeof details?.score === 'number' &&
    Number.isFinite(details.score) &&
    details.score > 0
      ? Math.round(details.score * 100)
      : null

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm transition-opacity"
      onClick={(e) => {
        if (e.target === e.currentTarget) {
          onClose()
        }
      }}
    >
      <div
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="toxic-modal-title"
        aria-describedby="toxic-modal-desc"
        className="relative bg-white dark:bg-zinc-900 border border-slate-200 dark:border-zinc-800 shadow-2xl rounded-2xl max-w-md w-full overflow-hidden text-left transform transition-all animate-in fade-in zoom-in-95 duration-200"
      >
        {/* Accent top gradient stripe */}
        <div className="h-1.5 bg-gradient-to-r from-orange-500 via-red-500 to-rose-600" />

        <div className="p-6">
          <div className="flex items-start gap-4">
            {/* Warning Icon Badge */}
            <div className="shrink-0 w-12 h-12 rounded-xl bg-red-100 dark:bg-red-950/60 border border-red-200 dark:border-red-900/60 flex items-center justify-center text-red-600 dark:text-red-400 text-2xl shadow-inner select-none">
              ⚠️
            </div>

            <div className="grow">
              <h3
                id="toxic-modal-title"
                className="text-lg font-bold text-slate-900 dark:text-white"
              >
                {title}
              </h3>
              <p
                id="toxic-modal-desc"
                className="mt-2 text-sm leading-relaxed text-slate-600 dark:text-slate-300"
              >
                {message}
              </p>
            </div>
          </div>

          {/* Community Guidelines callout */}
          <div className="mt-4 p-3 rounded-lg bg-orange-50 dark:bg-orange-950/40 border border-orange-200 dark:border-orange-900/40 text-xs text-orange-900 dark:text-orange-200 flex items-start gap-2.5">
            <span className="text-base select-none">🛡️</span>
            <div>
              <span className="font-semibold">Community Guidelines:</span>{' '}
              ConCord is committed to keeping chat welcoming for all users.
              Hostile, abusive, or toxic language is not permitted.
            </div>
          </div>

          {confidencePercent !== null && (
            <p className="mt-3 text-xs text-slate-400 dark:text-zinc-500 text-right">
              Automated moderation score: {confidencePercent}%
            </p>
          )}

          {/* Modal Action Buttons */}
          <div className="mt-6 flex justify-end gap-3">
            <button
              ref={dismissBtnRef}
              type="button"
              onClick={onClose}
              className="w-full sm:w-auto px-5 py-2.5 bg-orange-500 hover:bg-orange-600 dark:bg-orange-600 dark:hover:bg-orange-700 text-white font-medium text-sm rounded-lg shadow-sm hover:shadow transition-all focus:outline-none focus:ring-2 focus:ring-orange-500 focus:ring-offset-2 dark:focus:ring-offset-zinc-900 cursor-pointer"
            >
              Understood
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
