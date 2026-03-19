(() => {
  const $ = (sel, root = document) => root.querySelector(sel);
  const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

  const state = {
    data: null,
    warnings: [],
    enums: {
      FactionType: [],
      InteractionEffectType: []
    },
    dirty: false
  };

  const fileInput = $('#fileInput');
  const resetBtn = $('#resetBtn');
  const downloadBtn = $('#downloadBtn');
  const previewBtn = $('#previewBtn');
  const statusSummary = $('#statusSummary');
  const warningList = $('#warningList');
  const previewBox = $('#previewBox');

  const npcList = $('#npcList');
  const interactionList = $('#interactionList');
  const lawList = $('#lawList');
  const fieldDetailsList = $('#fieldDetailsList');
  const postsList = $('#postsList');
  const opinionsList = $('#opinionsList');

  const addNpcBtn = $('#addNpcBtn');
  const addInteractionBtn = $('#addInteractionBtn');
  const addLawBtn = $('#addLawBtn');
  const addPostGroupBtn = $('#addPostGroupBtn');
  const addOpinionBtn = $('#addOpinionBtn');

  const tabs = $$('.tab');
  const panels = $$('.panel');

  // ---- Utilities --------------------------------------------------------

  function markDirty() {
    state.dirty = true;
    if (state.data) {
      setStatus('Ready to download', 'ready');
      downloadBtn.disabled = false;
      previewBtn.disabled = false;
    }
    refreshWarnings();
  }

  function setStatus(text, variant = 'idle') {
    statusSummary.textContent = text;
    statusSummary.className = `status status--${variant}`;
  }

  function createField(label, value, onChange, opts = {}) {
    const wrapper = document.createElement('div');
    wrapper.className = 'field';
    const lbl = document.createElement('label');
    lbl.textContent = label;
    const input = opts.multiline ? document.createElement('textarea') : document.createElement('input');
    if (!opts.multiline) input.type = opts.type || 'text';
    input.className = 'input';
    if (opts.placeholder) input.placeholder = opts.placeholder;
    if (opts.multiline) {
      input.value = value || '';
    } else {
      input.value = value ?? '';
    }
    input.addEventListener('input', (e) => onChange(opts.number ? parseNumber(e.target.value, value) : e.target.value));
    wrapper.append(lbl, input);
    return wrapper;
  }

  function createSelect(label, value, options, onChange) {
    const wrapper = document.createElement('div');
    wrapper.className = 'field';
    const lbl = document.createElement('label');
    lbl.textContent = label;
    const select = document.createElement('select');
    options.forEach(opt => {
      const o = document.createElement('option');
      o.value = opt.value;
      o.textContent = `${opt.name} (${opt.value})`;
      if (`${opt.value}` === `${value}`) o.selected = true;
      select.appendChild(o);
    });
    select.addEventListener('change', (e) => onChange(parseInt(e.target.value, 10)));
    wrapper.append(lbl, select);
    return wrapper;
  }

  function createListControls(label, onAdd) {
    const row = document.createElement('div');
    row.className = 'list-controls';
    const tag = document.createElement('span');
    tag.className = 'section-tag';
    tag.textContent = label;
    const btn = document.createElement('button');
    btn.className = 'mini-btn';
    btn.type = 'button';
    btn.textContent = 'Add';
    btn.addEventListener('click', onAdd);
    row.append(tag, btn);
    return row;
  }

  function createMiniButtons({ onDuplicate, onDelete, onMoveUp, onMoveDown }) {
    const row = document.createElement('div');
    row.className = 'list-controls';
    const btns = [
      { text: '↑', handler: onMoveUp },
      { text: '↓', handler: onMoveDown },
      { text: 'Duplicate', handler: onDuplicate },
      { text: 'Delete', handler: onDelete, danger: true }
    ];
    btns.forEach(({ text, handler, danger }) => {
      const b = document.createElement('button');
      b.className = 'mini-btn';
      if (danger) b.classList.add('danger');
      b.type = 'button';
      b.textContent = text;
      b.addEventListener('click', handler);
      row.appendChild(b);
    });
    return row;
  }

  function parseNumber(value, fallback = 0) {
    const n = value === '' ? NaN : Number(value);
    return Number.isFinite(n) ? n : fallback;
  }

  function cloneDeep(obj) {
    return JSON.parse(JSON.stringify(obj));
  }

  function moveInPlace(arr, from, to) {
    if (to < 0 || to >= arr.length) return;
    const [item] = arr.splice(from, 1);
    arr.splice(to, 0, item);
  }

  function ensureDataShape(data) {
    return {
      fieldDetails: data.fieldDetails || [],
      npcs: data.npcs || [],
      interactions: data.interactions || [],
      laws: data.laws || [],
      posts: data.posts || [],
      opinions: data.opinions || []
    };
  }

  // ---- Tabs -------------------------------------------------------------

  tabs.forEach(tab => {
    tab.addEventListener('click', () => {
      const target = tab.getAttribute('data-target');
      tabs.forEach(t => t.classList.toggle('is-active', t === tab));
      panels.forEach(p => p.classList.toggle('is-active', p.id === `panel-${target}`));
      if (target === 'preview') renderPreview();
    });
  });

  // ---- Rendering --------------------------------------------------------

  function renderAll() {
    renderFieldDetails();
    renderNPCs();
    renderInteractions();
    renderLaws();
    renderPosts();
    renderOpinions();
    refreshWarnings();
    updateActionState();
  }

  function renderFieldDetails() {
    fieldDetailsList.innerHTML = '';
    if (!state.data) return;
    state.data.fieldDetails.forEach(fd => {
      const card = document.createElement('div');
      card.className = 'card';
      const title = document.createElement('h4');
      title.textContent = fd.enumName;
      const used = document.createElement('p');
      used.className = 'muted';
      used.textContent = `Used in ${fd.usedIn}`;
      const chips = document.createElement('div');
      chips.className = 'row';
      (fd.values || []).forEach(v => {
        const chip = document.createElement('span');
        chip.className = 'chip';
        chip.textContent = `${v.name} (${v.value})`;
        chips.appendChild(chip);
      });
      card.append(title, used, chips);
      fieldDetailsList.appendChild(card);
    });
  }

  function renderNPCs() {
    npcList.innerHTML = '';
    if (!state.data) return;
    const { npcs } = state.data;
    if (!npcs.length) {
      npcList.textContent = 'No NPCs yet. Add one to begin.';
      return;
    }
    npcs.forEach((npc, idx) => {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      const title = document.createElement('div');
      title.innerHTML = `<h4>${npc.name || 'Untitled NPC'}</h4><p class="muted">${npc.id || 'No id set'}</p>`;
      header.append(title);
      card.appendChild(header);

      const inputs = document.createElement('div');
      inputs.className = 'inputs';
      inputs.append(
        createField('ID (unique, referenced by interactions)', npc.id, (v) => { npc.id = v; markDirty(); }),
        createField('Display name', npc.name, (v) => { npc.name = v; markDirty(); }),
        createField('Prefab path', npc.prefabPath, (v) => { npc.prefabPath = v; markDirty(); }, { placeholder: 'Prefabs/NPCs/...' })
      );

      // orientations
      const orientWrap = document.createElement('div');
      orientWrap.className = 'field';
      const orientLabel = document.createElement('label');
      orientLabel.textContent = 'Personal values (orientations)';
      orientWrap.appendChild(orientLabel);
      const orientList = document.createElement('div');
      orientList.className = 'stack';
      (npc.orientations || []).forEach((val, oIdx) => {
        const row = document.createElement('div');
        row.className = 'row';
        row.appendChild(createSelect('Value', val, state.enums.FactionType, (nv) => { npc.orientations[oIdx] = nv; markDirty(); }));
        const del = document.createElement('button');
        del.className = 'mini-btn danger';
        del.type = 'button';
        del.textContent = 'Remove';
        del.addEventListener('click', () => {
          npc.orientations.splice(oIdx, 1);
          markDirty();
          renderNPCs();
        });
        row.appendChild(del);
        orientList.appendChild(row);
      });
      const addOrient = document.createElement('button');
      addOrient.className = 'mini-btn';
      addOrient.type = 'button';
      addOrient.textContent = 'Add orientation';
      addOrient.addEventListener('click', () => {
        npc.orientations = npc.orientations || [];
        npc.orientations.push(state.enums.FactionType[0]?.value ?? 0);
        markDirty();
        renderNPCs();
      });
      orientWrap.append(orientList, addOrient);
      inputs.appendChild(orientWrap);
      card.appendChild(inputs);

      card.appendChild(createMiniButtons({
        onDuplicate: () => {
          npcs.splice(idx + 1, 0, cloneDeep(npc));
          markDirty();
          renderNPCs();
        },
        onDelete: () => {
          npcs.splice(idx, 1);
          markDirty();
          renderNPCs();
        },
        onMoveUp: () => { moveInPlace(npcs, idx, idx - 1); markDirty(); renderNPCs(); },
        onMoveDown: () => { moveInPlace(npcs, idx, idx + 1); markDirty(); renderNPCs(); }
      }));

      npcList.appendChild(card);
    });
  }

  function renderInteractions() {
    interactionList.innerHTML = '';
    if (!state.data) return;
    const { interactions } = state.data;
    if (!interactions.length) {
      interactionList.textContent = 'No interactions yet. Add one to begin.';
      return;
    }
    const npcIds = state.data.npcs.map(n => n.id);
    interactions.forEach((inter, idx) => {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      const title = document.createElement('div');
      title.innerHTML = `<h4>${inter.name || 'Interaction'}</h4><p class="muted">${inter.npcId || 'No NPC linked'}</p>`;
      header.append(title);
      card.appendChild(header);

      const inputs = document.createElement('div');
      inputs.className = 'inputs';
      inputs.append(
        createField('Name', inter.name, (v) => { inter.name = v; markDirty(); }),
        createField('NPC id', inter.npcId, (v) => { inter.npcId = v; markDirty(); }, { placeholder: `One of: ${npcIds.join(', ')}` })
      );

      // dialogue
      const dialogWrap = document.createElement('div');
      dialogWrap.className = 'field';
      const dialogLabel = document.createElement('label');
      dialogLabel.textContent = 'Dialogue lines';
      dialogWrap.appendChild(dialogLabel);
      const dialogList = document.createElement('div');
      dialogList.className = 'stack';
      (inter.dialogue || []).forEach((line, dIdx) => {
        const row = document.createElement('div');
        row.className = 'row';
        const area = document.createElement('textarea');
        area.value = line;
        area.addEventListener('input', (e) => { inter.dialogue[dIdx] = e.target.value; markDirty(); });
        const del = document.createElement('button');
        del.className = 'mini-btn danger';
        del.type = 'button';
        del.textContent = 'Remove';
        del.addEventListener('click', () => {
          inter.dialogue.splice(dIdx, 1);
          markDirty();
          renderInteractions();
        });
        row.append(area, del);
        dialogList.appendChild(row);
      });
      const addLine = document.createElement('button');
      addLine.className = 'mini-btn';
      addLine.type = 'button';
      addLine.textContent = 'Add line';
      addLine.addEventListener('click', () => {
        inter.dialogue = inter.dialogue || [];
        inter.dialogue.push('');
        markDirty();
        renderInteractions();
      });
      dialogWrap.append(dialogList, addLine);
      inputs.appendChild(dialogWrap);

      // effects
      const effectWrap = document.createElement('div');
      effectWrap.className = 'field';
      const effectLabel = document.createElement('label');
      effectLabel.textContent = 'Effects';
      effectWrap.appendChild(effectLabel);
      const effectList = document.createElement('div');
      effectList.className = 'stack';
      (inter.effects || []).forEach((eff, eIdx) => {
        const row = document.createElement('div');
        row.className = 'row';
        row.append(
          createSelect('Type', eff.type, state.enums.InteractionEffectType, (nv) => { eff.type = nv; markDirty(); }),
          createField('Value', eff.value, (nv) => { eff.value = parseNumber(nv, eff.value); markDirty(); }, { type: 'number', number: true })
        );
        const del = document.createElement('button');
        del.className = 'mini-btn danger';
        del.type = 'button';
        del.textContent = 'Remove';
        del.addEventListener('click', () => {
          inter.effects.splice(eIdx, 1);
          markDirty();
          renderInteractions();
        });
        row.appendChild(del);
        effectList.appendChild(row);
      });
      const addEffect = document.createElement('button');
      addEffect.className = 'mini-btn';
      addEffect.type = 'button';
      addEffect.textContent = 'Add effect';
      addEffect.addEventListener('click', () => {
        inter.effects = inter.effects || [];
        inter.effects.push({ type: state.enums.InteractionEffectType[0]?.value ?? 0, value: 0 });
        markDirty();
        renderInteractions();
      });
      effectWrap.append(effectList, addEffect);
      inputs.appendChild(effectWrap);

      card.appendChild(inputs);
      card.appendChild(createMiniButtons({
        onDuplicate: () => { interactions.splice(idx + 1, 0, cloneDeep(inter)); markDirty(); renderInteractions(); },
        onDelete: () => { interactions.splice(idx, 1); markDirty(); renderInteractions(); },
        onMoveUp: () => { moveInPlace(interactions, idx, idx - 1); markDirty(); renderInteractions(); },
        onMoveDown: () => { moveInPlace(interactions, idx, idx + 1); markDirty(); renderInteractions(); }
      }));

      interactionList.appendChild(card);
    });
  }

  function renderLawEffects(list, target, enumOptions, valueLabel, typeKey = 'type') {
    target.innerHTML = '';
    (list || []).forEach((eff, idx) => {
      const row = document.createElement('div');
      row.className = 'row';
      row.append(
        createSelect('Type', eff[typeKey], enumOptions, (nv) => { eff[typeKey] = nv; markDirty(); }),
        createField(valueLabel, eff.value, (nv) => { eff.value = parseNumber(nv, eff.value); markDirty(); }, { type: 'number', number: true })
      );
      const del = document.createElement('button');
      del.className = 'mini-btn danger';
      del.type = 'button';
      del.textContent = 'Remove';
      del.addEventListener('click', () => {
        list.splice(idx, 1);
        markDirty();
        renderLaws();
      });
      row.appendChild(del);
      target.appendChild(row);
    });
  }

  function renderInteractionNames(entry, container) {
    container.innerHTML = '';
    const allNames = (state.data?.interactions || []).map(i => i.name);
    (entry.interactionNames || []).forEach((name, idx) => {
      const row = document.createElement('div');
      row.className = 'row';
      const input = document.createElement('input');
      input.className = 'input';
      input.value = name;
      input.placeholder = `One of: ${allNames.slice(0, 8).join(', ')}...`;
      input.addEventListener('input', (e) => { entry.interactionNames[idx] = e.target.value; markDirty(); });
      const del = document.createElement('button');
      del.className = 'mini-btn danger';
      del.type = 'button';
      del.textContent = 'Remove';
      del.addEventListener('click', () => { entry.interactionNames.splice(idx, 1); markDirty(); renderLaws(); });
      row.append(input, del);
      container.appendChild(row);
    });
  }

  function renderLaws() {
    lawList.innerHTML = '';
    if (!state.data) return;
    const { laws } = state.data;
    if (!laws.length) {
      lawList.textContent = 'No laws yet. Add one to begin.';
      return;
    }
    laws.forEach((law, idx) => {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      const title = document.createElement('div');
      title.innerHTML = `<h4>${law.name || 'Law'}</h4><p class="muted">${law.description || 'No short description'}</p>`;
      const iconNote = document.createElement('span');
      iconNote.className = 'badge';
      iconNote.textContent = 'iconPath is WIP in-game';
      header.append(title, iconNote);
      card.appendChild(header);

      const inputs = document.createElement('div');
      inputs.className = 'inputs';
      inputs.append(
        createField('Name', law.name, (v) => { law.name = v; markDirty(); }),
        createField('Description', law.description, (v) => { law.description = v; markDirty(); }),
        createField('Long description', law.longDescription, (v) => { law.longDescription = v; markDirty(); }, { multiline: true }),
        createField('Icon path (WIP)', law.iconPath, (v) => { law.iconPath = v; markDirty(); }, { placeholder: 'Sprites/...' })
      );

      // Effects
      const effectsWrap = document.createElement('div');
      effectsWrap.className = 'field';
      effectsWrap.appendChild(document.createElement('label')).textContent = 'Effects (personal value shifts)';
      const effList = document.createElement('div');
      effList.className = 'stack';
      renderLawEffects(law.effects || [], effList, state.enums.FactionType, 'Value');
      const addEff = document.createElement('button');
      addEff.className = 'mini-btn';
      addEff.type = 'button';
      addEff.textContent = 'Add effect';
      addEff.addEventListener('click', () => {
        law.effects = law.effects || [];
        law.effects.push({ type: state.enums.FactionType[0]?.value ?? 0, value: 0 });
        markDirty();
        renderLaws();
      });
      effectsWrap.append(effList, addEff);
      inputs.appendChild(effectsWrap);

      // Welfare effects
      const welfareWrap = document.createElement('div');
      welfareWrap.className = 'field';
      welfareWrap.appendChild(document.createElement('label')).textContent = 'Welfare effects';
      const welfareList = document.createElement('div');
      welfareList.className = 'stack';
      renderLawEffects(law.welfareEffects || [], welfareList, [
        { name: 'GDP', value: 0 },
        { name: 'Gini', value: 1 },
        { name: 'HumanCapital', value: 2 },
        { name: 'LifeExpectancy', value: 3 }
      ], 'Value (can be float)', 'indicator');
      const addW = document.createElement('button');
      addW.className = 'mini-btn';
      addW.type = 'button';
      addW.textContent = 'Add welfare effect';
      addW.addEventListener('click', () => {
        law.welfareEffects = law.welfareEffects || [];
        law.welfareEffects.push({ indicator: 0, value: 0 });
        markDirty();
        renderLaws();
      });
      welfareWrap.append(welfareList, addW);
      inputs.appendChild(welfareWrap);

      // Interaction names
      const interWrap = document.createElement('div');
      interWrap.className = 'field';
      interWrap.appendChild(document.createElement('label')).textContent = 'Interaction names (must exist)';
      const interList = document.createElement('div');
      interList.className = 'stack';
      renderInteractionNames(law, interList);
      const addInter = document.createElement('button');
      addInter.className = 'mini-btn';
      addInter.type = 'button';
      addInter.textContent = 'Add interaction name';
      addInter.addEventListener('click', () => {
        law.interactionNames = law.interactionNames || [];
        law.interactionNames.push('');
        markDirty();
        renderLaws();
      });
      interWrap.append(interList, addInter);
      inputs.appendChild(interWrap);

      card.appendChild(inputs);
      card.appendChild(createMiniButtons({
        onDuplicate: () => { laws.splice(idx + 1, 0, cloneDeep(law)); markDirty(); renderLaws(); },
        onDelete: () => { laws.splice(idx, 1); markDirty(); renderLaws(); },
        onMoveUp: () => { moveInPlace(laws, idx, idx - 1); markDirty(); renderLaws(); },
        onMoveDown: () => { moveInPlace(laws, idx, idx + 1); markDirty(); renderLaws(); }
      }));

      lawList.appendChild(card);
    });
  }

  function renderPosts() {
    postsList.innerHTML = '';
    if (!state.data) return;
    const postsData = state.data.posts || [];
    if (!postsData.length) {
      postsList.textContent = 'No posts yet. Add one to begin.';
      return;
    }
    postsData.forEach((block, idx) => {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      const title = document.createElement('div');
      title.innerHTML = `<h4>Law: ${block.lawName || 'Unset'}</h4><p class="muted">${(block.posts || []).length} posts</p>`;
      header.append(title);
      card.appendChild(header);

      const inputs = document.createElement('div');
      inputs.className = 'inputs';
      inputs.append(createField('Law name (must match law)', block.lawName, (v) => { block.lawName = v; markDirty(); }));

      const postList = document.createElement('div');
      postList.className = 'stack';
      (block.posts || []).forEach((p, pIdx) => {
        const pCard = document.createElement('div');
        pCard.className = 'card';
        const pInputs = document.createElement('div');
        pInputs.className = 'inputs';
        pInputs.append(
          createField('Author', p.author?.name, (v) => { p.author = p.author || {}; p.author.name = v; markDirty(); }),
          createField('Faction (text)', p.faction, (v) => { p.faction = v; markDirty(); }, { placeholder: 'GREEN / LIBERAL / ...' }),
          createField('Content', p.content, (v) => { p.content = v; markDirty(); }, { multiline: true }),
          createField('Image path', p.imagePath, (v) => { p.imagePath = v; markDirty(); })
        );

        // comments
        const commentsWrap = document.createElement('div');
        commentsWrap.className = 'field';
        commentsWrap.appendChild(document.createElement('label')).textContent = 'Comments';
        const cList = document.createElement('div');
        cList.className = 'stack';
        (p.comments || []).forEach((c, cIdx) => {
          const cRow = document.createElement('div');
          cRow.className = 'card';
          const cInputs = document.createElement('div');
          cInputs.className = 'inputs';
          cInputs.append(
            createField('Author', c.author?.name, (v) => { c.author = c.author || {}; c.author.name = v; markDirty(); }),
            createField('Content', c.content, (v) => { c.content = v; markDirty(); }),
            createField('Likes', c.reaction?.likes ?? 0, (v) => { c.reaction = c.reaction || { likes: 0, dislikes: 0 }; c.reaction.likes = parseNumber(v, 0); markDirty(); }, { type: 'number', number: true }),
            createField('Dislikes', c.reaction?.dislikes ?? 0, (v) => { c.reaction = c.reaction || { likes: 0, dislikes: 0 }; c.reaction.dislikes = parseNumber(v, 0); markDirty(); }, { type: 'number', number: true })
          );
          const cDel = document.createElement('button');
          cDel.className = 'mini-btn danger';
          cDel.type = 'button';
          cDel.textContent = 'Remove comment';
          cDel.addEventListener('click', () => {
            p.comments.splice(cIdx, 1);
            markDirty();
            renderPosts();
          });
          cRow.append(cInputs, cDel);
          cList.appendChild(cRow);
        });
        const addComment = document.createElement('button');
        addComment.className = 'mini-btn';
        addComment.type = 'button';
        addComment.textContent = 'Add comment';
        addComment.addEventListener('click', () => {
          p.comments = p.comments || [];
          p.comments.push({ author: { name: '' }, content: '', reaction: { likes: 0, dislikes: 0 } });
          markDirty();
          renderPosts();
        });
        commentsWrap.append(cList, addComment);
        pInputs.append(commentsWrap);

        pCard.appendChild(pInputs);
        pCard.appendChild(createMiniButtons({
          onDuplicate: () => { block.posts.splice(pIdx + 1, 0, cloneDeep(p)); markDirty(); renderPosts(); },
          onDelete: () => { block.posts.splice(pIdx, 1); markDirty(); renderPosts(); },
          onMoveUp: () => { moveInPlace(block.posts, pIdx, pIdx - 1); markDirty(); renderPosts(); },
          onMoveDown: () => { moveInPlace(block.posts, pIdx, pIdx + 1); markDirty(); renderPosts(); }
        }));

        postList.appendChild(pCard);
      });
      const addInnerPost = document.createElement('button');
      addInnerPost.className = 'mini-btn';
      addInnerPost.type = 'button';
      addInnerPost.textContent = 'Add post';
      addInnerPost.addEventListener('click', () => {
        block.posts = block.posts || [];
        block.posts.push({ author: { name: '' }, faction: '', content: '', imagePath: '', comments: [] });
        markDirty();
        renderPosts();
      });
      inputs.append(postList, addInnerPost);
      card.appendChild(inputs);

      card.appendChild(createMiniButtons({
        onDuplicate: () => { postsData.splice(idx + 1, 0, cloneDeep(block)); markDirty(); renderPosts(); },
        onDelete: () => { postsData.splice(idx, 1); markDirty(); renderPosts(); },
        onMoveUp: () => { moveInPlace(postsData, idx, idx - 1); markDirty(); renderPosts(); },
        onMoveDown: () => { moveInPlace(postsData, idx, idx + 1); markDirty(); renderPosts(); }
      }));

      postsList.appendChild(card);
    });
  }

  function renderOpinions() {
    opinionsList.innerHTML = '';
    if (!state.data) return;
    const { opinions } = state.data;
    if (!opinions.length) {
      opinionsList.textContent = 'No opinions yet. Add one to begin.';
      return;
    }
    opinions.forEach((op, idx) => {
      const card = document.createElement('div');
      card.className = 'card';
      const header = document.createElement('div');
      header.className = 'card__header';
      const title = document.createElement('div');
      title.innerHTML = `<h4>${op.lawName || 'Law name'}</h4><p class="muted">Pro / neutral / against takes</p>`;
      header.append(title);
      card.appendChild(header);

      const inputs = document.createElement('div');
      inputs.className = 'inputs';
      inputs.append(
        createField('Law name (must match law)', op.lawName, (v) => { op.lawName = v; markDirty(); }),
        createField('Pro', op.pro, (v) => { op.pro = v; markDirty(); }, { multiline: true }),
        createField('Neutral', op.neutral, (v) => { op.neutral = v; markDirty(); }, { multiline: true }),
        createField('Against', op.against, (v) => { op.against = v; markDirty(); }, { multiline: true })
      );
      card.appendChild(inputs);

      card.appendChild(createMiniButtons({
        onDuplicate: () => { opinions.splice(idx + 1, 0, cloneDeep(op)); markDirty(); renderOpinions(); },
        onDelete: () => { opinions.splice(idx, 1); markDirty(); renderOpinions(); },
        onMoveUp: () => { moveInPlace(opinions, idx, idx - 1); markDirty(); renderOpinions(); },
        onMoveDown: () => { moveInPlace(opinions, idx, idx + 1); markDirty(); renderOpinions(); }
      }));

      opinionsList.appendChild(card);
    });
  }

  function renderPreview() {
    if (!state.data) {
      previewBox.textContent = 'Load a JSON file to see the preview.';
      return;
    }
    const text = JSON.stringify(state.data, null, 2);
    const max = 6000;
    previewBox.textContent = text.length > max ? text.slice(0, max) + '\n... (truncated)' : text;
  }

  function updateActionState() {
    const hasData = !!state.data;
    downloadBtn.disabled = !hasData;
    previewBtn.disabled = !hasData;
  }

  // ---- Validation -------------------------------------------------------

  function refreshWarnings() {
    if (!state.data) {
      state.warnings = [];
      warningList.innerHTML = '';
      setStatus('Waiting for file...', 'idle');
      return;
    }
    const warnings = [];
    const enums = state.enums;

    const requiredEnums = ['FactionType', 'InteractionEffectType'];
    requiredEnums.forEach(name => {
      if (!enums[name]?.length) warnings.push(`Missing enum values for ${name} (fieldDetails).`);
    });

    const npcIds = new Set();
    const interactionNames = new Set();
    const lawNames = new Set();

    (state.data.npcs || []).forEach(n => {
      if (!n.id) warnings.push('NPC missing id.');
      if (npcIds.has(n.id)) warnings.push(`Duplicate NPC id "${n.id}".`);
      if (n.id) npcIds.add(n.id);
    });
    (state.data.interactions || []).forEach(i => {
      if (!i.name) warnings.push('Interaction missing name.');
      if (interactionNames.has(i.name)) warnings.push(`Duplicate interaction name "${i.name}".`);
      if (i.name) interactionNames.add(i.name);
    });
    (state.data.laws || []).forEach(l => {
      if (!l.name) warnings.push('Law missing name.');
      if (lawNames.has(l.name)) warnings.push(`Duplicate law name "${l.name}".`);
      if (l.name) lawNames.add(l.name);
    });

    (state.data.interactions || []).forEach(inter => {
      if (inter.npcId && !npcIds.has(inter.npcId)) warnings.push(`Interaction "${inter.name}" references missing NPC "${inter.npcId}".`);
      (inter.effects || []).forEach(e => {
        if (!enums.InteractionEffectType.some(v => v.value === e.type)) warnings.push(`Interaction "${inter.name}" has effect type "${e.type}" outside InteractionEffectType.`);
      });
    });

    (state.data.laws || []).forEach(law => {
      (law.effects || []).forEach(e => {
        if (!enums.FactionType.some(v => v.value === e.type)) warnings.push(`Law "${law.name}" has effect type "${e.type}" outside FactionType.`);
      });
      (law.welfareEffects || []).forEach(e => {
        if (![0, 1, 2, 3].includes(e.indicator)) warnings.push(`Law "${law.name}" has welfare indicator "${e.indicator}" outside expected range (0-3).`);
      });
      (law.interactionNames || []).forEach(n => {
        if (n && !interactionNames.has(n)) warnings.push(`Law "${law.name}" references missing interaction "${n}".`);
      });
    });

    (state.data.posts || []).forEach(block => {
      if (block.lawName && !lawNames.has(block.lawName)) warnings.push(`Posts block for "${block.lawName}" has no matching law.`);
    });

    (state.data.opinions || []).forEach(op => {
      if (op.lawName && !lawNames.has(op.lawName)) warnings.push(`Opinion for "${op.lawName}" has no matching law.`);
    });

    state.warnings = warnings;
    warningList.innerHTML = '';
    warnings.slice(0, 6).forEach(w => {
      const item = document.createElement('div');
      item.className = 'warning';
      item.textContent = w;
      warningList.appendChild(item);
    });
    if (warnings.length > 6) {
      const more = document.createElement('div');
      more.className = 'warning';
      more.textContent = `+${warnings.length - 6} more warnings`;
      warningList.appendChild(more);
    }
    setStatus(warnings.length ? 'Warnings detected' : 'All links look good', warnings.length ? 'warn' : 'ready');
  }

  // ---- Data loading -----------------------------------------------------

  function extractEnums(data) {
    const map = { FactionType: [], InteractionEffectType: [] };
    (data.fieldDetails || []).forEach(fd => {
      if (fd.enumName === 'FactionType') map.FactionType = fd.values || [];
      if (fd.enumName === 'InteractionEffectType') map.InteractionEffectType = fd.values || [];
    });
    return map;
  }

  async function loadFile(file) {
    const text = await file.text();
    let parsed;
    try {
      parsed = JSON.parse(text);
    } catch (err) {
      setStatus('Invalid JSON file', 'warn');
      warningList.innerHTML = `<div class="warning">Parse error: ${err.message}</div>`;
      return;
    }
    state.data = ensureDataShape(parsed);
    state.enums = extractEnums(state.data);
    state.dirty = false;
    setStatus('Loaded. Review sections to edit.', 'ready');
    renderAll();
  }

  // ---- Events -----------------------------------------------------------

  fileInput.addEventListener('change', (e) => {
    const file = e.target.files?.[0];
    if (file) loadFile(file);
  });

  resetBtn.addEventListener('click', () => {
    state.data = null;
    state.warnings = [];
    state.dirty = false;
    setStatus('Waiting for file...', 'idle');
    [npcList, interactionList, lawList, fieldDetailsList, postsList, opinionsList].forEach(el => el.innerHTML = '');
    warningList.innerHTML = '';
    previewBox.textContent = '';
    downloadBtn.disabled = true;
    previewBtn.disabled = true;
  });

  addNpcBtn.addEventListener('click', () => {
    if (!state.data) return;
    state.data.npcs.push({ id: '', name: '', prefabPath: '', orientations: [] });
    markDirty();
    renderNPCs();
  });

  addInteractionBtn.addEventListener('click', () => {
    if (!state.data) return;
    state.data.interactions.push({ name: '', npcId: '', dialogue: [], effects: [] });
    markDirty();
    renderInteractions();
  });

  addLawBtn.addEventListener('click', () => {
    if (!state.data) return;
    state.data.laws.push({ name: '', description: '', longDescription: '', iconPath: '', effects: [], welfareEffects: [], interactionNames: [] });
    markDirty();
    renderLaws();
  });

  addPostGroupBtn.addEventListener('click', () => {
    if (!state.data) return;
    state.data.posts.push({ lawName: '', posts: [] });
    markDirty();
    renderPosts();
  });

  addOpinionBtn.addEventListener('click', () => {
    if (!state.data) return;
    state.data.opinions.push({ lawName: '', pro: '', neutral: '', against: '' });
    markDirty();
    renderOpinions();
  });

  downloadBtn.addEventListener('click', () => {
    if (!state.data) return;
    const blob = new Blob([JSON.stringify(state.data, null, 2)], { type: 'application/json' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = 'game_database.json';
    a.click();
    URL.revokeObjectURL(a.href);
  });

  previewBtn.addEventListener('click', () => {
    renderPreview();
    tabs.forEach(t => t.classList.toggle('is-active', t.dataset.target === 'preview'));
    panels.forEach(p => p.classList.toggle('is-active', p.id === 'panel-preview'));
  });

  // initial state
  setStatus('Waiting for file...', 'idle');
})();
